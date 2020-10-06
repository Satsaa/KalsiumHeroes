using System.Collections;
using System.Collections.Generic;
using Muc.Editor;
using UnityEngine;

public abstract class Ability : ScriptableObject, IEventHandler<Events.Ability> {

  public string displayName;
  public string description;

  [Tooltip("The type of the ability. Physical = Disabled by 'Disarm', applicable damage reduced by the 'Defense' stat. Spell = Disabled by 'Silence', applicable damage reduced by 'Resistance' stat.")]
  public AbilityType type;

  [Tooltip("What types of targets can this ability target?"), EnumMask]
  public TargetType targetType;
  [Tooltip("The cast range of the ability.")]
  public Attribute<int> range = new Attribute<int>(1);

  [Tooltip("The shapes of the area this ability affects."), EnumMask]
  public EffectShape effectShape;
  public Attribute<int> effectShapeSize = new Attribute<int>(1);
  public Attribute<int> effectShapeDistance = new Attribute<int>(1);

  [Tooltip("How many turns it takes for this ability to be usable again.")]
  public Attribute<int> cooldown;

  [Tooltip("How many limited uses does the ability have. -1 = Unlimited uses")]
  public Attribute<int> uses = new Attribute<int>(-1);


  /// <summary> Returns a list of valid target tiles </summary>
  public virtual IEnumerable<GameHex> GetTargets(GameHex hex) {
    yield return null; // Todo: default calc
  }

  /// <summary> Returns a list of affected tiles if the Ability is cast on hex </summary>
  public virtual IEnumerable<GameHex> GetArea(GameHex hex) {
    yield return null; // Todo: default calc
  }


  public abstract bool EventIsFinished();
  public abstract bool SkipEvent();
  public abstract void StartEvent(Events.Ability data);
}
