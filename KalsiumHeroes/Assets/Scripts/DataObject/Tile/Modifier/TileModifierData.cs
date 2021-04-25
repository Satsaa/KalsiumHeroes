
using System;
using IHas;
using Muc.Editor;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(TileModifierData), menuName = "DataSources/" + nameof(TileModifierData))]
public class TileModifierData : ModifierData, IHasDisplayName, IHasDescription {

	public override Type createTypeConstraint => typeof(TileModifier);

	[Tooltip("Display name of the TileModifier.")]
	public TextSource displayName;
	TextSource IHasDisplayName.displayName => displayName;

	[Tooltip("Description of the TileModifier.")]
	public TextSource description;
	TextSource IHasDescription.description => description;

}
