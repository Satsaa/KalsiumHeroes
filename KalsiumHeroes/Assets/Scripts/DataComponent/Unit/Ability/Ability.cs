
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : UnitModifier, IOnTurnStart_Unit, IOnAbilityCastStart_Unit, IOnAnimationEventEnd {

	public AbilityData abilityData => (AbilityData)data;
	public override Type dataType => typeof(AbilityData);

	[HideInInspector, SerializeField] bool castBlocked = false;
	[HideInInspector, SerializeField] bool sendOnCastEnd = false;

	public abstract EventHandler<Events.Ability> CreateEventHandler(Events.Ability msg);

	public virtual void OnTurnStart() {
		castBlocked = false;
		abilityData.cooldown.value--;
		if (abilityData.cooldown.value <= 0) {
			abilityData.charges.value++;
			if (abilityData.cooldown.value <= 0 && abilityData.cooldown.other <= 0) abilityData.charges.ResetValue();
			else abilityData.charges.LimitValue();
			abilityData.cooldown.ResetValue();
		}
	}

	public virtual void OnAbilityCastStart(Ability ability) {
		if (ability.abilityData.abilityType == AbilityType.Base) return;
		if (!abilityData.alwaysCastable.value) {
			castBlocked = true;
		}
	}

	public virtual void OnAnimationEventEnd() {
		if (sendOnCastEnd) {
			sendOnCastEnd = false;
			unit.onEvents.Execute<IOnAbilityCastEnd_Unit>(v => v.OnAbilityCastEnd(this));
			unit.tile.onEvents.Execute<IOnAbilityCastEnd_Tile>(v => v.OnAbilityCastEnd(this));
			Game.onEvents.Execute<IOnAbilityCastEnd_Global>(v => v.OnAbilityCastEnd(this));
		}
	}

	/// <summary> Called when the ability is actually cast. </summary>
	public virtual void OnCast() {
		sendOnCastEnd = true;
		if (abilityData.uses.enabled) abilityData.uses.value--;
		if (abilityData.charges.value == abilityData.charges.other) abilityData.cooldown.ResetValue();
		if (abilityData.cooldown.value <= 0 && abilityData.cooldown.other <= 0) abilityData.charges.ResetValue();
		else abilityData.charges.value--;
		unit.unitData.energy.value -= abilityData.energyCost.value;
		unit.RefreshEnergy();

		unit.onEvents.Execute<IOnAbilityCastStart_Unit>(v => v.OnAbilityCastStart(this));
		unit.tile.onEvents.Execute<IOnAbilityCastStart_Tile>(v => v.OnAbilityCastStart(this));
		Game.onEvents.Execute<IOnAbilityCastStart_Global>(v => v.OnAbilityCastStart(this));
	}

	public float GetCalculatedDamage(float damage, DamageType damageType) {
		var abilityType = this.abilityData.abilityType;
		switch (abilityType) {
			default:
			case AbilityType.Base:
				Debug.LogWarning($"Unexpected {nameof(AbilityType)} {abilityType.ToString()}.");
				break;
			case AbilityType.Skill:
				damage *= 1f + unit.unitData.amps.skill.value;
				break;
			case AbilityType.WeaponSkill:
				damage *= 1f + unit.unitData.amps.weaponSkill.value;
				break;
			case AbilityType.Spell:
				damage *= 1f + unit.unitData.amps.spell.value;
				break;
		}

		switch (damageType) {
			case DamageType.Physical:
				damage *= 1f + unit.unitData.amps.physical.value;
				break;
			case DamageType.Magical:
				damage *= 1f + unit.unitData.amps.magical.value;
				break;
			case DamageType.Pure:
				damage *= 1f + unit.unitData.amps.pure.value;
				break;
			default:
				Debug.LogWarning($"Unexpected {nameof(DamageType)} {damageType.ToString()}.");
				return damage;
		}
		damage = unit.onEvents.Aggregate<IOnGetCalculatedAbilityDamage_Unit, float>(damage, (cur, v) => v.OnGetCalculatedAbilityDamage(cur, this, damageType));
		damage = unit.tile.onEvents.Aggregate<IOnGetCalculatedAbilityDamage_Tile, float>(damage, (cur, v) => v.OnGetCalculatedAbilityDamage(cur, this, damageType));
		damage = Game.onEvents.Aggregate<IOnGetCalculatedAbilityDamage_Global, float>(damage, (cur, v) => v.OnGetCalculatedAbilityDamage(cur, this, damageType));
		return Mathf.Max(0, damage);
	}


	#region IsReady

	/// <summary> Is the Ability castable at the moment? </summary>
	public virtual bool IsReady() {
		if (castBlocked) return false;
		if (abilityData.uses.enabled && abilityData.uses.value <= 0) return false;
		if (abilityData.energyCost.value > unit.unitData.energy.value) return false;
		if (abilityData.abilityType == AbilityType.Spell && unit.silenced.value) return false;
		if (abilityData.abilityType == AbilityType.WeaponSkill && unit.disarmed.value) return false;
		if (abilityData.charges.value > 0) return true;
		return false;
	}

	#endregion

	/// <summary>
	/// Returns a list of affected Tiles if the Ability is cast on tile.
	/// Used for highlighting and should be used when casting the ability (ensures equivalency).
	/// </summary>
	public virtual IEnumerable<Tile> GetAffectedArea(Tile tile) {
		return Game.grid.Radius(tile, abilityData.radius.value);
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
		if (abilityData.range.enabled) {
			return (abilityData.rangeMode) switch {
				RangeMode.Distance => Game.grid.Radius(tile, abilityData.range.value),

				RangeMode.PathDistance => Pathing.GetDistanceField(tile, abilityData.range.value, Pathers.Unphased).tiles.Keys,
				RangeMode.PathDistancePhased => Pathing.GetDistanceField(tile, abilityData.range.value, Pathers.Phased).tiles.Keys,
				RangeMode.PathDistanceFlying => Pathing.GetDistanceField(tile, abilityData.range.value, Pathers.Flying).tiles.Keys,
				RangeMode.PathDistancePhasedFlying => Pathing.GetDistanceField(tile, abilityData.range.value, Pathers.FlyingPhased).tiles.Keys,

				RangeMode.PathCost => Pathing.GetCostField(tile, maxCost: abilityData.range.value, pather: Pathers.Unphased).tiles.Keys,
				RangeMode.PathCostPhased => Pathing.GetCostField(tile, maxCost: abilityData.range.value, pather: Pathers.Phased).tiles.Keys,
				RangeMode.PathCostFlying => Pathing.GetCostField(tile, maxCost: abilityData.range.value, pather: Pathers.Flying).tiles.Keys,
				RangeMode.PathCostPhasedFlying => Pathing.GetCostField(tile, maxCost: abilityData.range.value, pather: Pathers.FlyingPhased).tiles.Keys,

				_ => Game.grid.Radius(tile, abilityData.range.value),
			};
		} else {
			return Game.grid.tiles.Values;
		}
	}

	/// <summary> Filter the target hexes with this method. Call the base implementation for default behaviour. </summary>
	protected virtual void GetTargets_FilterTargets(Tile tile, ref IEnumerable<Tile> targets) {

		// If ground not included. When ground is included all hexes in range are valid, so we need no filtering.
		if ((abilityData.targetType & TargetType.Ground) == 0) {

			bool self = (abilityData.targetType & TargetType.Self) != 0;
			bool ally = (abilityData.targetType & TargetType.Ally) != 0;
			bool enemy = (abilityData.targetType & TargetType.Enemy) != 0;
			bool neutral = (abilityData.targetType & TargetType.Neutral) != 0;

			if (abilityData.targetType > 0) {
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

		if (abilityData.requiresVision.value) targets = targets.Where(h => Game.grid.HasSight(tile, h));
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