
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : UnitModifier, IOnTurnStart_Unit, IOnAnimationEventEnd {

	public new AbilityData source => (AbilityData)_source;
	public new AbilityData data => (AbilityData)_data;
	public override Type dataType => typeof(AbilityData);

	[HideInInspector, SerializeField] public bool isCasting = false;

	public abstract EventHandler<Events.Ability> CreateEventHandler(Events.Ability msg);

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


	#region IsReady

	/// <summary> Is the Ability castable at the moment? </summary>
	public virtual bool IsReady() {
		if (data.uses.enabled && data.uses.value <= 0) return false;
		if (data.energyCost.value > unit.data.energy.value) return false;
		if (data.abilityType == AbilityType.Spell && unit.silenced.value) return false;
		if (data.abilityType == AbilityType.WeaponSkill && unit.disarmed.value) return false;
		if (data.charges.value > 0) return true;
		return false;
	}

	#endregion

	/// <summary>
	/// Returns a list of affected Tiles if the Ability is cast on tile.
	/// Used for highlighting and should be used when casting the ability (ensures equivalency).
	/// </summary>
	public virtual IEnumerable<Tile> GetAffectedArea(Tile tile) {
		return Game.grid.Radius(tile, data.radius.value);
	}

	#region GetTargets

	/// <summary> Returns a list of valid target Tiles. </summary>
	public virtual IEnumerable<Tile> GetTargets() {
		IEnumerable<Tile> res = GetTargets_GetRangeTargets(unit.tile);
		GetTargets_FilterTargets(unit.tile, ref res);
		return res;
	}

	/// <summary> Get the target Tiles in range with this method. Call the base implementation for default behaviour. </summary>
	protected virtual IEnumerable<Tile> GetTargets_GetRangeTargets(Tile tile) {
		if (data.range.enabled) {
			return (data.rangeMode) switch {
				RangeMode.Distance => Game.grid.Radius(tile, data.range.value),

				RangeMode.PathDistance => Pathing.GetDistanceField(tile, data.range.value, Pathers.Unphased).tiles.Keys,
				RangeMode.PathDistancePhased => Pathing.GetDistanceField(tile, data.range.value, Pathers.Phased).tiles.Keys,
				RangeMode.PathDistanceFlying => Pathing.GetDistanceField(tile, data.range.value, Pathers.Flying).tiles.Keys,
				RangeMode.PathDistancePhasedFlying => Pathing.GetDistanceField(tile, data.range.value, Pathers.FlyingPhased).tiles.Keys,

				RangeMode.PathCost => Pathing.GetCostField(tile, maxCost: data.range.value, pather: Pathers.Unphased).tiles.Keys,
				RangeMode.PathCostPhased => Pathing.GetCostField(tile, maxCost: data.range.value, pather: Pathers.Phased).tiles.Keys,
				RangeMode.PathCostFlying => Pathing.GetCostField(tile, maxCost: data.range.value, pather: Pathers.Flying).tiles.Keys,
				RangeMode.PathCostPhasedFlying => Pathing.GetCostField(tile, maxCost: data.range.value, pather: Pathers.FlyingPhased).tiles.Keys,

				_ => Game.grid.Radius(tile, data.range.value),
			};
		} else {
			return Game.grid.tiles.Values;
		}
	}

	/// <summary> Filter the target hexes with this method. Call the base implementation for default behaviour. </summary>
	protected virtual void GetTargets_FilterTargets(Tile tile, ref IEnumerable<Tile> targets) {

		// If ground not included. When ground is included all hexes in range are valid, so we need no filtering.
		if ((data.targetType & TargetType.Ground) == 0) {

			bool self = (data.targetType & TargetType.Self) != 0;
			bool ally = (data.targetType & TargetType.Ally) != 0;
			bool enemy = (data.targetType & TargetType.Enemy) != 0;
			bool neutral = (data.targetType & TargetType.Neutral) != 0;

			if (data.targetType > 0) {
				targets = targets.Where(h => {
					if (h.unit != null) {
						if (self && h.unit == unit) return true;
						if (ally && h.unit.team == unit.team && h.unit != this.unit) return true;
						if (enemy && h.unit.team != unit.team) return true;
						if (neutral && h.unit.team == Team.Neutral && h.unit != this.unit) return true;
					}
					return false;
				});
			}
		}

		if (data.requiresVision.value) targets = targets.Where(h => Game.grid.HasSight(tile, h));
	}

	#endregion


	#region GetTargeter

	/// <summary> Returns a targeter with onComplete and onCancel callbacks. </summary>
	public virtual Targeter GetTargeter() {
		return new AbilityTargeter(unit, this,
			onComplete: (targeter) => PostDefaultAbilityEvent(targeter.selections[0])
		);
	}

	protected void PostDefaultAbilityEvent(Tile target) {
		Game.client.PostEvent(new Events.Ability() {
			ability = data.identifier,
			targets = new Vector3Int[] { target.hex.pos },
			unit = unit.tile.hex.pos
		});
	}

	#endregion
}