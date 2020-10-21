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

  /// <summary>
  /// Returns a list of affected tiles if the Ability is cast on hex.
  /// Used for highlighting and should be used when casting the ability (ensures equivalency).
  /// </summary>
  public virtual IEnumerable<GameHex> GetAreaOfEffect(GameHex hex) {
    yield return hex;
  }

  /// <summary> Returns a list of valid target tiles </summary>
  public virtual IEnumerable<GameHex> GetTargets() {
    var hex = unit.hex;

    IEnumerable<GameHex> res = new List<GameHex>(0);
    switch (abilityData.rangeMode) {
      default:
      case RangeMode.Distance:
        res = Game.grid.Radius(hex, abilityData.range.value);
        break;
      case RangeMode.PathDistance:
        res = Game.grid.GetDistanceField(hex, distance: abilityData.range.value).distances.Keys;
        break;
      case RangeMode.PathCost:
        res = Game.grid.GetCostField(hex, maxCost: abilityData.range.value).costs.Keys;
        break;
    }

    // If ground not included. When ground is included all hexes in range are valid, so we need no filtering.
    if ((abilityData.targetType & TargetType.Ground) == 0) {

      bool self = (abilityData.targetType & TargetType.Self) != 0;
      bool ally = (abilityData.targetType & TargetType.Ally) != 0;
      bool enemy = (abilityData.targetType & TargetType.Enemy) != 0;
      bool neutral = (abilityData.targetType & TargetType.Neutral) != 0;

      if (self || ally || enemy) {
        res = res.Where(h => {
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
      res.Where(h => Game.grid.HasSight(hex, h));
    }

    return res;
  }


  /// <summary> Returns a targeting sequence with onComplete and onCancel callbacks </summary>
  public virtual Targeter GetTargeter() {
    return new AbilityTargeter(unit, this,
      onComplete: (seq) => {
        Debug.Log("Sequence complete! Posting event.");
        Game.client.PostEvent(new Events.Ability() {
          ability = data.identifier,
          target = seq.selection[0].hex.pos,
          unit = unit.hex.hex.pos
        });
      },
      onCancel: (seq) => {
        Debug.Log("We got cancelled :(");
      }
    );
  }

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