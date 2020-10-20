using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Mathf;

[CreateAssetMenu(fileName = nameof(UnitData), menuName = "DataSources/" + nameof(UnitData))]
public class UnitData : ScriptableObject {

  [Tooltip("Display name of the Unit. E.g. \"Oracle\"")]
  public string displayName;
  [Tooltip("String identifier of this Unit. E.g. \"unit_oracle\"")]
  public string identifier;
  [Tooltip("Description displayed to users.")]
  public string description;

  [Tooltip("Speed determines when the unit gets to be played. The higher the speed, the higher the priority within any turn.")]
  public Attribute<int> speed = new Attribute<int>(1);

  [Tooltip("Determines how many tiles any unit can move per turn.")]
  public Attribute<int> movement = new Attribute<int>(1);

  [Tooltip("The health of the unit. Reach 0 and the unit dies.")]
  public Attribute<float> health = new Attribute<float>(100);

  [Tooltip("The maximum health of the unit.")]
  public Attribute<float> maxHealth = new Attribute<float>(100);

  [Tooltip("The amount of resistance to physical damage the unit posesses.")]
  public Attribute<int> defense;

  [Tooltip("The amount of resistance to magical damage the unit posesses.")]
  public Attribute<int> resistance;

  // public UnitModifierData[] modifiers;
  // public AbilityData[] abilities;
  // public StatusEffectData[] effects;
}