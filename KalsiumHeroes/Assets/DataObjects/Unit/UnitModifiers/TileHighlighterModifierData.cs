using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(TileHighlighterModifierData), menuName = "DataSources/UnitModifiers/" + nameof(TileHighlighterModifierData))]
public class TileHighlighterModifierData : UnitModifierData {

	public override Type createTypeConstraint => typeof(TileHighlighterModifier);

	[Tooltip("The color of the tile highlighted under this unit if its on the player's team.")]
	public Color ownTeamColor = Color.green;

	[Tooltip("The color of the tile highlighted under this unit if its not on the player's team.")]
	public Color otherTeamColor = Color.red;

	[Tooltip("Hide the highlight during animations like moving and such.")]
	public bool hideDuringAnimation = true;

}
