
using System;
using Muc.Editor;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(TileModifierData), menuName = "DataSources/" + nameof(TileModifierData))]
public class TileModifierData : ModifierData {

	public override Type createTypeConstraint => typeof(TileModifier);

}
