
using System;
using Muc.Editor;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(EdgeModifierData), menuName = "DataSources/" + nameof(EdgeModifierData))]
public class EdgeModifierData : ModifierData {

	public override Type ownerConstraint => typeof(EdgeModifier);

	[Header("Edge Modifier Data")]

	[Tooltip("Display name of the EdgeModifier.")]
	public TextSource displayName;

	[Tooltip("Description of the EdgeModifier.")]
	public TextSource description;

}
