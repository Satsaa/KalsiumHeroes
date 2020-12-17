
using UnityEngine;
using Muc.Data;

public abstract class DataComponentData : ScriptableObject {

	[Header("Data Component Data")]
	[Tooltip("Display name of this DataComponent, displayed to users. (\"Oracle\")")]
	public string displayName;

	[Tooltip("Description of this DataComponent, displayed to users.")]
	public string description;

	[Tooltip("String identifier of this DataComponent. (\"unit_oracle\")")]
	public string identifier;

	[Tooltip("Instantiate this Type of DataComponent for this data.")]
	public SerializedType<DataComponent> componentType;

}