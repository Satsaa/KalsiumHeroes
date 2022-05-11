using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Muc.Extensions;
using UnityEngine.UI;
using UnityEngine;

public class TileHighlighterModifier : UnitModifier, IOnTurnStart_Unit, IOnTurnEnd_Unit, IOnChangePosition_Unit, IOnAnimationEventStart, IOnAnimationEventEnd {

	[Tooltip("The color of the tile highlighted under this unit if its on the player's team.")]
	public Color ownTeamColor = Color.green;

	[Tooltip("The color of the tile highlighted under this unit if its not on the player's team.")]
	public Color otherTeamColor = Color.red;

	[Tooltip("Hide the highlight during animations like moving and such.")]
	public bool hideDuringAnimation = true;


	void IOnChangePosition_Unit.OnChangePosition(Tile from, Tile to) {
		if (unit.isCurrent) {
			Unhighlight(from);
			Highlight(to);
		}
	}

	void IOnTurnEnd_Unit.OnTurnEnd() {
		Unhighlight(unit.tile);
	}

	void IOnTurnStart_Unit.OnTurnStart() {
		Highlight(unit.tile);
	}

	void Highlight(Tile tile) {
		if (tile && (!hideDuringAnimation || !Game.events.animating)) {
			var color = unit.team == Game.instance.team ? ownTeamColor : otherTeamColor;
			tile.highlighter.Highlight(color, Highlighter.currentUnitPriority);
		}
	}

	void Unhighlight(Tile tile) {
		if (tile) {
			tile.highlighter.Unhighlight(Highlighter.currentUnitPriority);
		}
	}

	void IOnAnimationEventStart.OnAnimationEventStart(EventHandler handler) {
		if (hideDuringAnimation && unit.isCurrent) {
			Unhighlight(unit.tile);
		}
	}

	void IOnAnimationEventEnd.OnAnimationEventEnd() {
		if (hideDuringAnimation && unit.isCurrent) {
			Highlight(unit.tile);
		}
	}

}
