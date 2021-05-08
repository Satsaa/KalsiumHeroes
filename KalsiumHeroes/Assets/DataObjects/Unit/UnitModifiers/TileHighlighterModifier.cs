using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Muc.Extensions;
using UnityEngine.UI;
using UnityEngine;

public class TileHighlighterModifier : UnitModifier, IOnTurnStart_Unit, IOnTurnEnd_Unit, IOnChangePosition_Unit, IOnAnimationEventStart, IOnAnimationEventEnd {

	public new TileHighlighterModifierData data => (TileHighlighterModifierData)_data;
	public override Type dataType => typeof(TileHighlighterModifierData);

	protected override void OnCreate() {
		base.OnCreate();

	}

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
		if (tile && (!data.hideDuringAnimation || !Game.events.animating)) {
			var color = unit.team == Game.instance.team ? data.ownTeamColor : data.otherTeamColor;
			tile.highlighter.Highlight(color, Highlighter.currentUnitPriority);
		}
	}

	void Unhighlight(Tile tile) {
		if (tile) {
			tile.highlighter.Unhighlight(Highlighter.currentUnitPriority);
		}
	}

	void IOnAnimationEventStart.OnAnimationEventStart(EventHandler handler) {
		if (data.hideDuringAnimation && unit.isCurrent) {
			Unhighlight(unit.tile);
		}
	}

	void IOnAnimationEventEnd.OnAnimationEventEnd() {
		if (data.hideDuringAnimation && unit.isCurrent) {
			Highlight(unit.tile);
		}
	}

}
