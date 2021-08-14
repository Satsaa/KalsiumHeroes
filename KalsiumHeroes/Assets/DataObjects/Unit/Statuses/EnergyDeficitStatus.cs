using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnergyDeficitStatus : Status, IOnEnergyDeficit_Unit {

	public new EnergyDeficitStatusData data => (EnergyDeficitStatusData)_data;
	public override Type dataType => typeof(EnergyDeficitStatusData);

	[SerializeField] Attribute<int> stacks = new();
	[SerializeField] int stacksDuringOwnTurn;

	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);

		data.hidden.current.ConfigureAlterer(add, this,
			applier: (v, a) => v || a > 0, // Will not override if already hidden
			updater: () => stacks.current,
			updateEvents: new[] { stacks.current.onChanged }
		);

		unit.data.defense.current.ConfigureAlterer(add, this,
			applier: (v, a) => v + a,
			updater: () => data.defenseChange.current * stacks.current,
			updateEvents: new[] { data.defenseChange.current.onChanged, stacks.current.onChanged }
		);

		unit.data.resistance.current.ConfigureAlterer(add, this,
			applier: (v, a) => v + a,
			updater: () => data.resistanceChange.current * stacks.current,
			updateEvents: new[] { data.resistanceChange.current.onChanged, stacks.current.onChanged }
		);

	}

	public void OnEnergyDeficit(int deficit) {
		if (unit.isCurrent) {
			stacksDuringOwnTurn += deficit;
		}
		Add(deficit);
	}

	public override void OnTurnEnd() {
		if (stacksDuringOwnTurn > 0) {
			stacks.current.value = stacksDuringOwnTurn;
		} else {
			Clear();
		}
		stacksDuringOwnTurn = 0;
	}

	private void Add(int excess) {
		stacks.current.value += excess;
		if (!container) return;
		var vfx = container.GetComponent<VisualEffect>();
		if (vfx) vfx.Play();
		var pts = container.GetComponent<ParticleSystem>();
		if (pts) pts.Play();
	}
	private void Clear() {
		stacks.current.value = 0;
		if (!container) return;
		var vfx = container.GetComponent<VisualEffect>();
		if (vfx) vfx.Stop();
		var pts = container.GetComponent<ParticleSystem>();
		if (pts) pts.Stop();
	}

	public override void OnRoundStart() {
		base.OnRoundStart();
	}
}
