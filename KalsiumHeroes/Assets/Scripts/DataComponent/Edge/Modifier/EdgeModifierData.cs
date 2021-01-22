
using System;
using Muc.Editor;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(EdgeModifierData), menuName = "DataSources/" + nameof(EdgeModifierData))]
public class EdgeModifierData : ModifierData {

	public override Type createTypeConstraint => typeof(EdgeModifier);

	[Tooltip("Display name of the EdgeModifier.")]
	public TextSource displayName;

	[Tooltip("Description of the EdgeModifier.")]
	public TextSource description;

}
