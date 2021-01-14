using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnergyDeficitStatus : Status, IOnEnergyDeficit_Unit {

	public EnergyDeficitStatusData energyDeficitStatusData => (EnergyDeficitStatusData)data;
	public override Type dataType => typeof(EnergyDeficitStatusData);

	public int stacks;

	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);
		statusEffectData.hidden.ConfigureAlterer(add, v => stacks > 0);
		unit.unitData.defense.ConfigureAlterer(add, v => v - energyDeficitStatusData.defenseReduction * stacks);
		unit.unitData.resistance.ConfigureAlterer(add, v => v - energyDeficitStatusData.defenseReduction * stacks);
	}

	public void OnEnergyDeficit(int deficit) {
		Add(deficit);
	}

	public override void OnTurnEnd() {
		Clear();
	}

	private void Add(int excess) {
		this.stacks += excess;
		if (!modifierData.container) return;
		var vfx = GetComponent<VisualEffect>();
		if (vfx) vfx.Play();
		var pts = GetComponent<ParticleSystem>();
		if (pts) pts.Play();
	}
	private void Clear() {
		stacks = 0;
		if (!modifierData.container) return;
		var vfx = GetComponent<VisualEffect>();
		if (vfx) vfx.Stop();
		var pts = GetComponent<ParticleSystem>();
		if (pts) pts.Stop();
	}
}
