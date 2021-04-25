
using System;
using UnityEngine;
using IHas;

[CreateAssetMenu(fileName = nameof(EdgeModifierData), menuName = "DataSources/" + nameof(EdgeModifierData))]
public class EdgeModifierData : ModifierData, IHasDisplayName, IHasDescription {

	public override Type createTypeConstraint => typeof(EdgeModifier);

	[Tooltip("Display name of the EdgeModifier.")]
	public TextSource displayName;
	TextSource IHasDisplayName.displayName => displayName;

	[Tooltip("Description of the EdgeModifier.")]
	public TextSource description;
	TextSource IHasDescription.description => description;

}
