
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : UnitModifier, IOnTurnStart_Unit, IOnAnimationEventEnd {

	public new AbilityData source => (AbilityData)_source;
	public new AbilityData data => (AbilityData)_data;
	public override Type dataType => typeof(AbilityData);

	[HideInInspector, SerializeField] public bool isCasting;

	public abstract EventHandler<Events.Ability> CreateHandler(Events.Ability msg);

	public virtual void OnTurnStart() {
		data.cooldown.value--;
		if (data.cooldown.value <= 0) {
			data.charges.value++;
			if (data.cooldown.value <= 0 && data.cooldown.other <= 0) data.charges.ResetValue();
			else data.charges.LimitValue();
			data.cooldown.ResetValue();
		}
	}

	public virtual void OnAnimationEventEnd() {
		if (isCasting) {
			isCasting = false;
			using (var scope = new OnEvents.Scope()) {
				unit.onEvents.ForEach<IOnAbilityCastEnd_Unit>(scope, v => v.OnAbilityCastEnd(this));
				unit.tile.onEvents.ForEach<IOnAbilityCastEnd_Tile>(scope, v => v.OnAbilityCastEnd(this));
				Game.onEvents.ForEach<IOnAbilityCastEnd_Global>(scope, v => v.OnAbilityCastEnd(this));
			}
		}
	}

	/// <summary> Is the Ability castable at the moment? </summary>
	public virtual bool IsReady() {
		if (data.uses.enabled && data.uses.value <= 0) return false;
		if (data.energyCost.value > unit.data.energy.value) return false;
		if (data.abilityType == AbilityType.Spell && unit.data.silenced.value) return false;
		if (data.abilityType == AbilityType.WeaponSkill && unit.data.disarmed.value) return false;
		if (data.charges.value > 0) return true;
		return false;
	}

	/// <summary> Called when the ability is actually cast. </summary>
	public virtual void OnCast() {
		isCasting = true;
		if (data.uses.enabled) data.uses.value--;
		if (data.charges.value == data.charges.other) data.cooldown.ResetValue();
		if (data.cooldown.value <= 0 && data.cooldown.other <= 0) data.charges.ResetValue();
		else data.charges.value--;
		unit.data.energy.value -= data.energyCost.value;
		unit.RefreshEnergy();

		using (var scope = new OnEvents.Scope()) {
			unit.onEvents.ForEach<IOnAbilityCastStart_Unit>(scope, v => v.OnAbilityCastStart(this));
			unit.tile.onEvents.ForEach<IOnAbilityCastStart_Tile>(scope, v => v.OnAbilityCastStart(this));
			Game.onEvents.ForEach<IOnAbilityCastStart_Global>(scope, v => v.OnAbilityCastStart(this));
		}
	}

	/// <summary> Calculates damage and deals it to the target. </summary>
	protected override void DealDamage(Unit target, float damage, DamageType damageType) {
		CalculateDamage(ref damage, ref damageType);
		target.DealCalculatedDamage(this, damage, damageType);
	}

	/// <summary> Calculates damage for later use. Amplifications and modifiers are applied to the ref values. </summary>
	public void CalculateDamage(ref float damage, ref DamageType damageType) {
		var abilityType = this.data.abilityType;
		switch (abilityType) {
			default:
			case AbilityType.Base:
				Debug.LogWarning($"Unexpected {nameof(AbilityType)} {abilityType.ToString()}.");
				break;
			case AbilityType.Skill:
				damage *= 1f + unit.data.amps.skill.value;
				break;
			case AbilityType.WeaponSkill:
				damage *= 1f + unit.data.amps.weaponSkill.value;
				break;
			case AbilityType.Spell:
				damage *= 1f + unit.data.amps.spell.value;
				break;
		}

		switch (damageType) {
			case DamageType.Physical:
				damage *= 1f + unit.data.amps.physical.value;
				break;
			case DamageType.Magical:
				damage *= 1f + unit.data.amps.magical.value;
				break;
			case DamageType.Pure:
				damage *= 1f + unit.data.amps.pure.value;
				break;
			default:
				Debug.LogWarning($"Unexpected {nameof(DamageType)} {damageType.ToString()}.");
				break;
		}
		using (var scope = new OnEvents.Scope()) {
			var (_damage, _damageType) = (damage, damageType);
			unit.onEvents.ForEach<IOnCalculateDamage_Unit>(scope, v => v.OnCalculateDamage(this, ref _damage, ref _damageType));
			unit.tile.onEvents.ForEach<IOnCalculateDamage_Tile>(scope, v => v.OnCalculateDamage(this, ref _damage, ref _damageType));
			Game.onEvents.ForEach<IOnCalculateDamage_Global>(scope, v => v.OnCalculateDamage(this, ref _damage, ref _damageType));
			(damage, damageType) = (_damage, _damageType);
		}
	}

	/// <summary> Sends an Ability event to the server. </summary>
	public void PostDefaultAbilityEvent(params Tile[] targets) {
		Game.client.PostEvent(new Events.Ability() {
			casterTile = unit.tile.hex.pos,
			casterIndex = unit.tile.units.IndexOf(unit),
			abilityIndex = unit.modifiers.IndexOf<Ability>(this),
			targets = targets.Select(v => v.hex.pos).ToArray(),
			targetIndexes = targets.Select(v => 0).ToArray() // Todo unit target index
		});
	}

}