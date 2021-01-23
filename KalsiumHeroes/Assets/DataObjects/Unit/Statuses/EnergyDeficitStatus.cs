﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnergyDeficitStatus : Status, IOnEnergyDeficit_Unit {

	public new EnergyDeficitStatusData data => (EnergyDeficitStatusData)_data;
	public override Type dataType => typeof(EnergyDeficitStatusData);

	[SerializeField] int stacks;
	[SerializeField] int stacksDuringOwnTurn;
	bool ownTurn => Game.rounds.current == unit;

	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);
		data.hidden.ConfigureAlterer(add, v => stacks > 0);
		unit.data.defense.ConfigureAlterer(add, v => v - data.defenseReduction * stacks);
		unit.data.resistance.ConfigureAlterer(add, v => v - data.defenseReduction * stacks);
	}

	public void OnEnergyDeficit(int deficit) {
		if (ownTurn) {
			stacksDuringOwnTurn += deficit;
		}
		Add(deficit);
	}

	public override void OnTurnEnd() {
		if (stacksDuringOwnTurn > 0) {
			stacks = stacksDuringOwnTurn;
		} else {
			Clear();
		}
		stacksDuringOwnTurn = 0;
	}

	private void Add(int excess) {
		stacks += excess;
		if (!data.container) return;
		var vfx = container.GetComponent<VisualEffect>();
		if (vfx) vfx.Play();
		var pts = container.GetComponent<ParticleSystem>();
		if (pts) pts.Play();
	}
	private void Clear() {
		stacks = 0;
		if (!data.container) return;
		var vfx = container.GetComponent<VisualEffect>();
		if (vfx) vfx.Stop();
		var pts = container.GetComponent<ParticleSystem>();
		if (pts) pts.Stop();
	}
}