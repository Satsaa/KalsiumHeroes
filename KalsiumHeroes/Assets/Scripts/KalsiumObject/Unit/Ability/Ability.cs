
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : UnitModifier, IOnTurnStart_Unit, IOnAnimationEventEnd {

	[Tooltip("Type of the ability.")]
	public AbilityTypeAttr abilityType;

	[Tooltip("The amount of energy required to cast this ability.")]
	public EnergyCost energyCost;

	[Tooltip("How many turns it takes for this ability to gain a charge.")]
	public Cooldown cooldown;

	[Tooltip("How many charges does the ability have.")]
	public Charges charges;

	[Tooltip("How many times can the ability be cast in total.")]
	public Uses uses;

	[Tooltip("Can the unit move after this spell is cast?")]
	public AllowMove allowMove;

	[Serializable]
	public class AbilityTypeAttr : Attribute<AbilityType> {
		public override string identifier => "Attribute_Ability_AbilityType";
		public override string TooltipText(IAttribute source) => current == AbilityType.Base ? null : $"<style=prefix>{Format(source == this)}</style>";
		public override string Format(bool isSource) => Lang.GetStr($"{identifier}_{current.value}");
	}

	[Serializable]
	public class EnergyCost : Attribute<int> {
		public override string identifier => "Attribute_Ability_EnergyCost";
		public override string TooltipText(IAttribute source) {
			if (current != 0) return DefaultTooltip(source);
			return null;
		}
	}

	[Serializable]
	public class Cooldown : MaxAttribute<int> {
		Cooldown() : base(0, 1) { }
		public override string identifier => "Attribute_Ability_Cooldown";
		public override string TooltipText(IAttribute source) {
			if (max != 1) return DefaultTooltip(source);
			return null;
		}
	}

	[Serializable]
	public class Charges : MaxAttribute<int> {
		Charges() : base(1, 1) { }
		public override string identifier => "Attribute_Ability_Charges";
		public override string TooltipText(IAttribute source) {
			if (max != 1) return DefaultTooltip(source);
			return null;
		}
	}

	[Serializable]
	public class Uses : ToggleAttribute<int> {
		Uses() : base(false) { }
		public override string identifier => "Attribute_Ability_Uses";
		public override string TooltipText(IAttribute source) {
			if (enabled) return DefaultTooltip(source);
			return null;
		}
	}

	[Serializable]
	public class AllowMove : Attribute<bool> {
		public override string identifier => "Attribute_Ability_AllowMove";
		public override string TooltipText(IAttribute source) {
			if (current) return DefaultTooltip(source);
			return null;
		}
	}


	[HideInInspector] public bool isCasting;

	public abstract EventHandler<GameEvents.Ability> CreateHandler(GameEvents.Ability msg);

	public virtual void OnTurnStart() {
		if (Game.rounds.round <= unit.spawnRound) return;
		cooldown.current.value--;
		if (cooldown.current <= 0) {
			charges.current.value++;
			if (cooldown.max <= 0) charges.Max();
			else charges.Ceil();
			cooldown.Max();
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
					Game.hooks.ForEach<IOnAbilityCastEnd_Game>(scope, v => v.OnAbilityCastEnd(this));
				}
			};
		}
	}

	/// <summary> Is the Ability castable at the moment? </summary>
	public virtual bool IsReady() {
		if (uses.enabled && uses.current <= 0) return false;
		if (energyCost.current > unit.energy.current) return false;
		if (abilityType.current == AbilityType.Spell && unit.silenced.current) return false;
		if (abilityType.current == AbilityType.WeaponSkill && unit.disarmed.current) return false;
		if (charges.current <= 0) return false;
		return true;
	}

	/// <summary> Called when the ability is actually cast. </summary>
	public virtual void OnCast() {
		isCasting = true;
		if (uses.enabled) uses.current.value--;
		if (charges.current == charges.max) cooldown.Max();
		if (cooldown.current <= 0 && cooldown.max <= 0) charges.Max();
		else charges.current.value--;
		unit.energy.current.value -= energyCost.current;
		unit.RefreshEnergy();

		using (var scope = new Hooks.Scope()) {
			unit.hooks.ForEach<IOnAbilityCastStart_Unit>(scope, v => v.OnAbilityCastStart(this));
			unit.tile.hooks.ForEach<IOnAbilityCastStart_Tile>(scope, v => v.OnAbilityCastStart(this));
			Game.hooks.ForEach<IOnAbilityCastStart_Game>(scope, v => v.OnAbilityCastStart(this));
		}
	}

	/// <summary> Add entry to combat log. </summary>
	public abstract string CombatLog(GameEvents.Ability msg);

	/// <summary> Calculates damage and deals it to the target. </summary>
	protected override void DealDamage(Unit target, float damage, DamageType damageType) {
		CalculateDamage(ref damage, ref damageType);
		target.DealCalculatedDamage(this, damage, damageType);
	}

	/// <summary> Calculates damage for later use. Amplifications and modifiers are applied to the ref values. </summary>
	public void CalculateDamage(ref float damage, ref DamageType damageType) {
		var abilityType = this.abilityType;
		using (var scope = new Hooks.Scope()) {
			var (_damage, _damageType) = (damage, damageType);
			unit.hooks.ForEach<IOnCalculateDamage_Unit>(scope, v => v.OnCalculateDamage(this, ref _damage, ref _damageType));
			unit.tile.hooks.ForEach<IOnCalculateDamage_Tile>(scope, v => v.OnCalculateDamage(this, ref _damage, ref _damageType));
			Game.hooks.ForEach<IOnCalculateDamage_Game>(scope, v => v.OnCalculateDamage(this, ref _damage, ref _damageType));
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