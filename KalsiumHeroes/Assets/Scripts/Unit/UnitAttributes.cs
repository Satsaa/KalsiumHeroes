
using UnityEngine;

public class UnitAttributes : ScriptableObject {

  [Min(0)]
  [Tooltip("Speed determines when the unit gets to be played. The higher the speed, the higher the priority within any turn.")]
  public Attribute<int> speed;

  [Min(0)]
  [Tooltip("Determines how many tiles any unit can move per turn.")]
  public Attribute<int> movement;

  [Min(0)]
  [Tooltip("The base health of the unit. Reach 0 and the unit dies")]
  public Attribute<float> health;

  [Range(0, 100)]
  [Tooltip("The amount of resistance to physical attacks the unit posesses")]
  public Attribute<int> defense;

  [Range(0, 100)]
  [Tooltip("The amount of resistance to magical attacks the unit posesses")]
  public Attribute<int> resistance;
}