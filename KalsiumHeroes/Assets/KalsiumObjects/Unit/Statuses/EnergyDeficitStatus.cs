using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = nameof(EnergyDeficitStatus), menuName = "KalsiumHeroes/Status/" + nameof(EnergyDeficitStatus))]
public class EnergyDeficitStatus : Status, IOnEnergyDeficit_Unit {

	[Tooltip("Amount of defense change per stack.")]
	public Attribute<int> defenseChange = new(-5);

	[Tooltip("Amount of resistance change per stack.")]
	public Attribute<int> resistanceChange = new(-5);


	[SerializeField] Attribute<int> stacks = new();
	[SerializeField] int stacksDuringOwnTurn;

	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);

		hidden.current.ConfigureAlterer(add, this,
			applier: (v, a) => v || a > 0, // Will not override if already hidden
			updater: () => stacks.current,
			updateEvents: new[] { stacks.current.onChanged }
		);

		unit.defense.current.ConfigureAlterer(add, this,
			applier: (v, a) => v + a,
			updater: () => defenseChange.current * stacks.current,
			updateEvents: new[] { defenseChange.current.onChanged, stacks.current.onChanged }
		);

		unit.resistance.current.ConfigureAlterer(add, this,
			applier: (v, a) => v + a,
			updater: () => resistanceChange.current * stacks.current,
			updateEvents: new[] { resistanceChange.current.onChanged, stacks.current.onChanged }
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

}
