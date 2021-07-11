
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

	public abstract EventHandler<GameEvents.Ability> CreateHandler(GameEvents.Ability msg);

	public virtual void OnTurnStart() {
		if (Game.rounds.round <= unit.spawnRound) return;
		data.cooldown.value.value--;
		if (data.cooldown.value <= 0) {
			data.charges.value.value++;
			if (data.cooldown.max <= 0) data.charges.Max();
			else data.charges.Ceil();
			data.cooldown.Max();
		}
	}

	public virtual void OnAnimationEventEnd() {
		if (isCasting) {
			isCasting = false;
			//!!! Is work? No onFinishEvent before
			Hooks.onFinishEvent += () => {
				using (var scope = new Hooks.Scope()) {
					unit.hooks.ForEach<IOnAbilityCastEnd_Unit>(scope, v => v.OnAbilityCastEnd(this));
					unit.tile.hooks.ForEach<IOnAbilityCastEnd_Tile>(scope, v => v.OnAbilityCastEnd(this));
					Game.hooks.ForEach<IOnAbilityCastEnd_Global>(scope, v => v.OnAbilityCastEnd(this));
				}
			};
		}
	}

	/// <summary> Is the Ability castable at the moment? </summary>
	public virtual bool IsReady() {
		if (data.uses.enabled && data.uses.value <= 0) return false;
		if (data.energyCost.value > unit.data.energy.value) return false;
		if (data.abilityType.value == AbilityType.Spell && unit.data.silenced.value) return false;
		if (data.abilityType.value == AbilityType.WeaponSkill && unit.data.disarmed.value) return false;
		if (data.charges.value <= 0) return false;
		return true;
	}

	/// <summary> Called when the ability is actually cast. </summary>
	public virtual void OnCast() {
		isCasting = true;
		if (data.uses.enabled) data.uses.value.value--;
		if (data.charges.value == data.charges.max) data.cooldown.Max();
		if (data.cooldown.value <= 0 && data.cooldown.max <= 0) data.charges.Max();
		else data.charges.value.value--;
		unit.data.energy.value.value -= data.energyCost.value;
		unit.RefreshEnergy();

		using (var scope = new Hooks.Scope()) {
			unit.hooks.ForEach<IOnAbilityCastStart_Unit>(scope, v => v.OnAbilityCastStart(this));
			unit.tile.hooks.ForEach<IOnAbilityCastStart_Tile>(scope, v => v.OnAbilityCastStart(this));
			Game.hooks.ForEach<IOnAbilityCastStart_Global>(scope, v => v.OnAbilityCastStart(this));
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
		using (var scope = new Hooks.Scope()) {
			var (_damage, _damageType) = (damage, damageType);
			unit.hooks.ForEach<IOnCalculateDamage_Unit>(scope, v => v.OnCalculateDamage(this, ref _damage, ref _damageType));
			unit.tile.hooks.ForEach<IOnCalculateDamage_Tile>(scope, v => v.OnCalculateDamage(this, ref _damage, ref _damageType));
			Game.hooks.ForEach<IOnCalculateDamage_Global>(scope, v => v.OnCalculateDamage(this, ref _damage, ref _damageType));
			(damage, damageType) = (_damage, _damageType);
		}
	}

	/// <summary> Sends an Ability event to the server. </summary>
	public void PostDefaultAbilityEvent(params Tile[] targets) {
		App.client.Post(new GameEvents.Ability() {
			casterTile = unit.tile.hex.pos,
			casterIndex = unit.tile.units.IndexOf(unit),
			abilityIndex = unit.modifiers.IndexOf<Ability>(this),
			targets = targets.Select(v => v.hex.pos).ToArray(),
			targetIndexes = targets.Select(v => 0).ToArray() // Todo unit target index
		});
	}

}