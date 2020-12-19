
using UnityEngine;
using Muc.Data;

public abstract class ModifierData : DataComponentData {

	[Header("Modifier Data")]
	[Tooltip("If defined, when creating this Modifier, instantiate this GameObject as a child and add the Modifier to it instead.")]
	public GameObject container;

}