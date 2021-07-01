using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnergyExcessStatus : Status, IOnEnergyExcess_Unit, IOnAbilityCastStart_Unit, IOnAbilityCastEnd_Unit, IOnCalculateDamage_Unit {

	public new EnergyExcessStatusData data => (EnergyExcessStatusData)_data;
	public override Type dataType => typeof(EnergyExcessStatusData);

	[HideInInspector, SerializeField] Attribute<int> stacks = new();
	[HideInInspector, SerializeField] bool didCalculateDamage;

	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);

		data.hidden.ConfigureValueAlterer(add, this,
			applier: (v, a) => v || a > 0, // Will not override if already hidden
			updater: () => stacks.value,
			updateEvents: new[] { stacks.onValueChanged }
		);

		unit.data.amps.weaponSkill.ConfigureValueAlterer(add, this,
			applier: (v, a) => v + a,
			updater: () => data.amps.weaponSkill.value * stacks.value,
			updateEvents: new[] { data.amps.weaponSkill.onValueChanged, stacks.onValueChanged }
		);
		unit.data.amps.spell.ConfigureValueAlterer(add, this,
			applier: (v, a) => v + a,
			updater: () => data.amps.spell.value * stacks.value,
			updateEvents: new[] { data.amps.weaponSkill.onValueChanged, stacks.onValueChanged }
		);
		unit.data.amps.skill.ConfigureValueAlterer(add, this,
			applier: (v, a) => v + a,
			updater: () => data.amps.skill.value * stacks.value,
			updateEvents: new[] { data.amps.weaponSkill.onValueChanged, stacks.onValueChanged }
		);

		unit.data.amps.pure.ConfigureValueAlterer(add, this,
			applier: (v, a) => v + a,
			updater: () => data.amps.pure.value * stacks.value,
			updateEvents: new[] { data.amps.weaponSkill.onValueChanged, stacks.onValueChanged }
		);
		unit.data.amps.physical.ConfigureValueAlterer(add, this,
			applier: (v, a) => v + a,
			updater: () => data.amps.physical.value * stacks.value,
			updateEvents: new[] { data.amps.weaponSkill.onValueChanged, stacks.onValueChanged }
		);
		unit.data.amps.magical.ConfigureValueAlterer(add, this,
			applier: (v, a) => v + a,
			updater: () => data.amps.magical.value * stacks.value,
			updateEvents: new[] { data.amps.weaponSkill.onValueChanged, stacks.onValueChanged }
		);
	}


	public void OnEnergyExcess(int excess) {
		Add(excess);
	}

	public override void OnTurnEnd() {
		Clear();
	}

	void IOnAbilityCastStart_Unit.OnAbilityCastStart(Ability ability) {
		didCalculateDamage = false;
	}

	void IOnCalculateDamage_Unit.OnCalculateDamage(Modifier source, ref float damage, ref DamageType type) {
		if (source is Ability) didCalculateDamage = true;
	}

	void IOnAbilityCastEnd_Unit.OnAbilityCastEnd(Ability ability) {
		if (didCalculateDamage) Clear();
	}

	private void Add(int excess) {
		stacks.value += excess;
		if (!data.container) return;
		var vfx = container.GetComponent<VisualEffect>();
		if (vfx) vfx.Play();
		var pts = container.GetComponent<ParticleSystem>();
		if (pts) pts.Play();
	}
	private void Clear() {
		stacks.value = 0;
		if (!data.container) return;
		var vfx = container.GetComponent<VisualEffect>();
		if (vfx) vfx.Stop();
		var pts = container.GetComponent<ParticleSystem>();
		if (pts) pts.Stop();
	}

	public override void OnRoundStart() {
		base.OnRoundStart();
		Debug.Log("ENERGYEXCESS STATUS ROUND: " + Game.rounds.round);
	}
}
