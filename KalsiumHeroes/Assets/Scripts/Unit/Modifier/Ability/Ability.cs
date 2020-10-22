#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : UnitModifier, IEventHandler<Events.Ability> {

  public AbilityData abilityData => (AbilityData)data;


  public abstract bool EventIsFinished();
  public abstract bool SkipEvent();
  public abstract void StartEvent(Events.Ability data);

  /// <summary> Is the Ability castable at the moment? </summary>
  public virtual bool IsReady() {
    if (abilityData.uses && abilityData.uses.value <= 0) return false;
    if (abilityData.cooldown && abilityData.cooldown.value > 0) return false;

    return true;
  }

  public override void OnTurnStart() {
    if (abilityData.cooldown) abilityData.cooldown.value--;
    base.OnTurnStart();
  }

  /// <summary>
  /// Returns a list of affected tiles if the Ability is cast on hex.
  /// Used for highlighting and should be used when casting the ability (ensures equivalency).
  /// </summary>
  public virtual IEnumerable<GameHex> GetAreaOfEffect(GameHex hex) {
    yield return hex;
  }

  /// <summary> Returns a list of valid target tiles </summary>
  public virtual IEnumerable<GameHex> GetTargets() {
    IEnumerable<GameHex> res = GetRangeTargets(unit.hex);
    FilterTargets(unit.hex, ref res);
    return res;
  }

  /// <summary> Get the target hexes in range with this method. Call the base implementation for default behaviour. </summary>
  protected virtual IEnumerable<GameHex> GetRangeTargets(GameHex hex) {
    if (abilityData.range) {
      switch (abilityData.rangeMode) {
        default:
        case RangeMode.Distance:
          return Game.grid.Radius(hex, abilityData.range.value);
        case RangeMode.PathDistance:
          return Game.grid.GetDistanceField(hex, distance: abilityData.range.value).distances.Keys;
        case RangeMode.PathCost:
          return Game.grid.GetCostField(hex, maxCost: abilityData.range.value).costs.Keys;
      }
    } else {
      return Game.grid.hexes.Values;
    }
  }

  /// <summary> Filter the target hexes with this method. Call the base implementation for default behaviour. </summary>
  protected virtual void FilterTargets(GameHex hex, ref IEnumerable<GameHex> targets) {

    // If ground not included. When ground is included all hexes in range are valid, so we need no filtering.
    if ((abilityData.targetType & TargetType.Ground) == 0) {

      bool self = (abilityData.targetType & TargetType.Self) != 0;
      bool ally = (abilityData.targetType & TargetType.Ally) != 0;
      bool enemy = (abilityData.targetType & TargetType.Enemy) != 0;
      bool neutral = (abilityData.targetType & TargetType.Neutral) != 0;

      if (self || ally || enemy) {
        targets = targets.Where(h => {
          if (h.unit != null) {
            if (self && h.unit == unit) return true;
            if (ally && h.unit.team == unit.team) return true;
            if (enemy && h.unit.team != unit.team) return true;
            if (neutral && h.unit.team == Team.Neutral) return true;
          }
          return false;
        });
      }
    }

    if (abilityData.requiresVision) {
      targets = targets.Where(h => Game.grid.HasSight(hex, h));
    }
  }


  /// <summary> Returns a targeter with onComplete and onCancel callbacks </summary>
  public virtual Targeter GetTargeter() {
    return new AbilityTargeter(unit, this,
      onComplete: OnTargeterComplete,
      onCancel: OnTargeterCancel
    );
  }

  protected virtual void OnTargeterComplete(Targeter targeter) {
    if (abilityData.uses) abilityData.uses.value--;
    if (abilityData.cooldown) abilityData.cooldown.value = abilityData.cooldown.other;
    PostEvent(targeter.selection[0]);
  }

  protected virtual void PostEvent(GameHex hex) {
    Game.client.PostEvent(new Events.Ability() {
      ability = data.identifier,
      target = hex.hex.pos,
      unit = unit.hex.hex.pos
    });
  }

  protected virtual void OnTargeterCancel(Targeter targeter) { }
}

#if UNITY_EDITOR

[CustomEditor(typeof(Ability), true)]
public class AbilityEditor : Editor {

  Ability t => (Ability)target;

  public override bool RequiresConstantRepaint() => true;


  public override void OnInspectorGUI() {
    serializedObject.Update();
    DrawDefaultInspector();

    using (new EditorGUI.DisabledGroupScope(!Game.events.finished)) {
      if (GUILayout.Button($"Cast")) {
        Debug.Log($"Success: {Game.targeting.TryStartSequence(t.GetTargeter())}");
      }
    }

    serializedObject.ApplyModifiedProperties();

  }

}
#endif