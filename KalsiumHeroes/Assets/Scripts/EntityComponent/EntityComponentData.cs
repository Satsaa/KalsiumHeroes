
using UnityEngine;

public abstract class EntityComponentData : ScriptableObject {

  [Header("Entity Component Data")]
  [Tooltip("Display name of this entity component, displayed to users. (\"Oracle\")")]
  public string displayName;

  [Tooltip("Description of this entity component, displayed to users.")]
  public string description;

  [Tooltip("String identifier of this entity component. (\"unit_oracle\")")]
  public string identifier;

  [Tooltip("The c# type that this data belongs to.")]
  public string type;

}