
using System;
using IHas;
using Muc.Editor;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(UnitModifierData), menuName = "DataSources/" + nameof(UnitModifierData))]
public class UnitModifierData : ModifierData, IHasDisplayName, IHasDescription, IHasSprite {

	public override Type createTypeConstraint => typeof(UnitModifier);

	[Tooltip("Display name of the UnitModifier.")]
	public TextSource displayName;
	TextSource IHasDisplayName.displayName => displayName;

	[Tooltip("Description of the UnitModifier.")]
	public TextSource description;
	TextSource IHasDescription.description => description;

	[Tooltip("Displayed image")]
	public Sprite sprite;
	Sprite IHasSprite.sprite => sprite;

}
