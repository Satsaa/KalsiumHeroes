
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : Modifier {

  public AbilityData abilityData => (AbilityData)data;
  public override Type dataType => typeof(AbilityData);

  [HideInInspector]
  public bool castBlocked = false;

  public abstract EventHandler<Events.Ability> CreateEventHandler(Events.Ability data);

  public override void OnTurnStart() {
    base.OnTurnStart();
    castBlocked = false;
    abilityData.cooldown.value--;
    if (abilityData.cooldown.value <= 0) {
      abilityData.charges.value++;
      if (abilityData.cooldown.value <= 0 && abilityData.cooldown.other <= 0) abilityData.charges.ResetValue();
      else abilityData.charges.LimitValue();
      abilityData.cooldown.ResetValue();
    }
  }

  public override void OnAbilityCast(Ability ability) {
    base.OnAbilityCast(ability);
    if (!abilityData.alwaysCastable.value) {
      castBlocked = true;
    }
  }

  /// <summary> Called when the ability is actually cast. </summary>
  public virtual void OnCast() {
    if (abilityData.uses.enabled) abilityData.uses.value--;
    if (abilityData.charges.value == abilityData.charges.other) abilityData.cooldown.ResetValue();
    if (abilityData.cooldown.value <= 0 && abilityData.cooldown.other <= 0) abilityData.charges.ResetValue();
    else abilityData.charges.value--;

    var isBase = abilityData.abilityType != AbilityType.Base;
    if (isBase) foreach (var modifier in unit.modifiers) modifier.OnAbilityCast(this);
    else foreach (var modifier in unit.modifiers) modifier.OnBaseAbilityCast(this);
  }


  #region IsReady

  /// <summary> Is the Ability castable at the moment? </summary>
  public virtual bool IsReady() {
    if (castBlocked) return false;
    if (abilityData.uses.enabled && abilityData.uses.value <= 0) return false;
    if (abilityData.abilityType == AbilityType.Spell && unit.silenced.value) return false;
    if (abilityData.abilityType == AbilityType.WeaponSkill && unit.disarmed.value) return false;
    if (abilityData.charges.value > 0) return true;
    return false;
  }

  #endregion

  /// <summary>
  /// Returns a list of affected tiles if the Ability is cast on hex.
  /// Used for highlighting and should be used when casting the ability (ensures equivalency).
  /// </summary>
  public virtual IEnumerable<GameHex> GetAffectedArea(GameHex hex) {
    return Game.grid.Radius(hex, abilityData.radius.value);
  }

  #region GetTargets

  /// <summary> Returns a list of valid target tiles </summary>
  public virtual IEnumerable<GameHex> GetTargets() {
    IEnumerable<GameHex> res = GetTargets_GetRangeTargets(unit.hex);
    GetTargets_FilterTargets(unit.hex, ref res);
    return res;
  }

  /// <summary> Get the target hexes in range with this method. Call the base implementation for default behaviour. </summary>
  protected virtual IEnumerable<GameHex> GetTargets_GetRangeTargets(GameHex hex) {
    if (abilityData.range.enabled) {
      switch (abilityData.rangeMode) {
        default:
        case RangeMode.Distance:
          return Game.grid.Radius(hex, abilityData.range.value);
        case RangeMode.PathDistance:
          return Game.grid.GetDistanceField(hex, abilityData.range.value, (h => !h.blocked && !h.unit)).distances.Keys;
        case RangeMode.PathDistancePassThrough:
          return Game.grid.GetDistanceField(hex, abilityData.range.value).distances.Keys;
        case RangeMode.PathCost:
          return Game.grid.GetCostField(hex, maxCost: abilityData.range.value, passable: (h => !h.blocked && !h.unit)).costs.Keys;
        case RangeMode.PathCostPassThrough:
          return Game.grid.GetCostField(hex, maxCost: abilityData.range.value).costs.Keys;
      }
    } else {
      return Game.grid.hexes.Values;
    }
  }

  /// <summary> Filter the target hexes with this method. Call the base implementation for default behaviour. </summary>
  protected virtual void GetTargets_FilterTargets(GameHex hex, ref IEnumerable<GameHex> targets) {

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

    if (abilityData.requiresVision.value) targets = targets.Where(h => Game.grid.HasSight(hex, h));
  }

  #endregion


  #region GetTargeter

  /// <summary> Returns a targeter with onComplete and onCancel callbacks. </summary>
  public virtual Targeter GetTargeter() {
    return new AbilityTargeter(unit, this,
      onComplete: (targeter) => PostDefaultAbilityEvent(targeter.selection[0])
    );
  }

  protected void PostDefaultAbilityEvent(GameHex target) {
    Game.client.PostEvent(new Events.Ability() {
      ability = data.identifier,
      target = target.hex.pos,
      unit = unit.hex.hex.pos
    });
  }

  #endregion
}