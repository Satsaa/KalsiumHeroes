using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Mathf;

[CreateAssetMenu(fileName = nameof(UnitData), menuName = "DataSources/" + nameof(UnitData))]
public class UnitData : EntityComponentData {

  [Header("Unit Data")]
  [Tooltip("Speed determines when the unit gets to be played. The higher the speed, the higher the priority within any turn.")]
  public Attribute<int> speed = new Attribute<int>(1);

  [Tooltip("Determines how many tiles any unit can move per turn.")]
  public Attribute<int> movement = new Attribute<int>(1);

  [AttributeLabels("Current", "Max")]
  [Tooltip("The health of the unit.")]
  public DualAttribute<float> health = new DualAttribute<float>(100, 100);

  [Tooltip("The amount of resistance to physical damage the unit posesses.")]
  public Attribute<int> defense;

  [Tooltip("The amount of resistance to magical damage the unit posesses.")]
  public Attribute<int> resistance;

}