using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnergyDeficitStatus : Status, IOnEnergyDeficit_Unit {

	public new EnergyDeficitStatusData data => (EnergyDeficitStatusData)_data;
	public override Type dataType => typeof(EnergyDeficitStatusData);

	[SerializeField] Attribute<int> stacks;
	[SerializeField] int stacksDuringOwnTurn;

	bool ownTurn => Game.rounds.unit == unit;

	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);

		data.hidden.value.ConfigureAlterer(add, this,
			applier: (v, a) => v || a > 0, // Will not override if already hidden
			updater: () => stacks.value,
			updateEvents: new[] { stacks.value.onChanged }
		);

		unit.data.defense.value.ConfigureAlterer(add, this,
			applier: (v, a) => v + a,
			updater: () => data.defenseChange.value * stacks.value,
			updateEvents: new[] { data.defenseChange.value.onChanged, stacks.value.onChanged }
		);

		unit.data.resistance.value.ConfigureAlterer(add, this,
			applier: (v, a) => v + a,
			updater: () => data.resistanceChange.value * stacks.value,
			updateEvents: new[] { data.resistanceChange.value.onChanged, stacks.value.onChanged }
		);

	}

	public void OnEnergyDeficit(int deficit) {
		if (ownTurn) {
			stacksDuringOwnTurn += deficit;
		}
		Add(deficit);
	}

	public override void OnTurnEnd() {
		if (stacksDuringOwnTurn > 0) {
			stacks.value.value = stacksDuringOwnTurn;
		} else {
			Clear();
		}
		stacksDuringOwnTurn = 0;
	}

	private void Add(int excess) {
		stacks.value.value += excess;
		if (!data.container) return;
		var vfx = container.GetComponent<VisualEffect>();
		if (vfx) vfx.Play();
		var pts = container.GetComponent<ParticleSystem>();
		if (pts) pts.Play();
	}
	private void Clear() {
		stacks.value.value = 0;
		if (!data.container) return;
		var vfx = container.GetComponent<VisualEffect>();
		if (vfx) vfx.Stop();
		var pts = container.GetComponent<ParticleSystem>();
		if (pts) pts.Stop();
	}

	public override void OnRoundStart() {
		base.OnRoundStart();
		Debug.Log("ENERGYDEFICIT STATUS ROUND: " + Game.rounds.round); // 8
	}
}
