
using UnityEngine;
using Muc.Data;

public abstract class DataComponentData : ScriptableObject {

	[Header("Entity Component Data")]
	[Tooltip("Display name of this entity component, displayed to users. (\"Oracle\")")]
	public string displayName;

	[Tooltip("Description of this entity component, displayed to users.")]
	public string description;

	[Tooltip("String identifier of this entity component. (\"unit_oracle\")")]
	public string identifier;

	[Tooltip("Instantiate this Type of EntityComponent for this data.")]
	public SerializedType<DataComponent> componentType;

}