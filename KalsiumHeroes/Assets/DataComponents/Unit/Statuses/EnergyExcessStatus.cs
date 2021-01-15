using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnergyExcessStatus : Status, IOnEnergyExcess_Unit, IOnAbilityCastStart_Unit, IOnAbilityCastEnd_Unit, IOnGetCalculatedAbilityDamage_Unit {

	public EnergyExcessStatusData energyExcessStatusData => (EnergyExcessStatusData)data;
	public override Type dataType => typeof(EnergyExcessStatusData);

	[HideInInspector, SerializeField] int stacks;
	[HideInInspector, SerializeField] bool didCalculateDamage;

	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);

		statusEffectData.hidden.ConfigureAlterer(add, v => stacks > 0);

		unit.unitData.amps.weaponSkill.ConfigureAlterer(add, v => v + energyExcessStatusData.amps.weaponSkill.value * stacks);
		unit.unitData.amps.spell.ConfigureAlterer(add, v => v + energyExcessStatusData.amps.spell.value * stacks);
		unit.unitData.amps.skill.ConfigureAlterer(add, v => v + energyExcessStatusData.amps.skill.value * stacks);

		unit.unitData.amps.pure.ConfigureAlterer(add, v => v + energyExcessStatusData.amps.pure.value * stacks);
		unit.unitData.amps.physical.ConfigureAlterer(add, v => v + energyExcessStatusData.amps.physical.value * stacks);
		unit.unitData.amps.magical.ConfigureAlterer(add, v => v + energyExcessStatusData.amps.magical.value * stacks);
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

	float IOnGetCalculatedAbilityDamage_Unit.OnGetCalculatedAbilityDamage(float damage, Ability ability, DamageType damageType) {
		didCalculateDamage = true;
		return damage;
	}

	void IOnAbilityCastEnd_Unit.OnAbilityCastEnd(Ability ability) {
		if (didCalculateDamage) Clear();
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
