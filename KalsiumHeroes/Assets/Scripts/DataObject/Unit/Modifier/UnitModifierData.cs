
using System;
using Muc.Editor;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(UnitModifierData), menuName = "DataSources/" + nameof(UnitModifierData))]
public class UnitModifierData : ModifierData {

	public override Type createTypeConstraint => typeof(UnitModifier);

	[Tooltip("Display name of the UnitModifier.")]
	public TextSource displayName;

	[Tooltip("Description of the UnitModifier.")]
	public TextSource description;

	[Tooltip("Displayed image")]
	public Sprite sprite;

}
