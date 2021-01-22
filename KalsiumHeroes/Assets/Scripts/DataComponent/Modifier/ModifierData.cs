
using System;
using UnityEngine;
using Muc.Data;

[CreateAssetMenu(fileName = nameof(ModifierData), menuName = "DataSources/" + nameof(ModifierData))]
public abstract class ModifierData : DataObjectData {

	public override Type ownerConstraint => typeof(Modifier);

	[Header("Modifier Data")]

	[Tooltip("If defined, when creating this Modifier, instantiate this GameObject as a child and add the Modifier to it instead.")]
	public GameObject container;

}