using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAbilityHandler : EventHandler<Events.Ability> {

	public MoveAbility creator;

	[SerializeField, HideInInspector] float animTime;
	[SerializeField, HideInInspector] bool animating;
	[SerializeField, HideInInspector] Tile start;
	[SerializeField, HideInInspector] Tile end;
	[SerializeField, HideInInspector] Tile[] path;
	[SerializeField, HideInInspector] int index;
	int maxIndex => path.Length - 1;

	public MoveAbilityHandler(Events.Ability data, MoveAbility creator) : base(data) {
		this.creator = creator;
		start = Game.grid.tiles[data.unit];
		end = Game.grid.tiles[data.target];
		Debug.Log("Handling move ability event!");
		if (end.unit || !end.tileData.passable.value) {
			Debug.LogError("Target Tile is unreachable!");
		} else {
			ExecuteOff(start);
			Pathing.CheapestPath(start, end, out this.path, out var field, Pathers.OneWayUnitBlocking);
			var cost = field.scores[field.closest];
			creator.usedMovement += cost + 1;
			animating = true;
			animTime = 0;
		}
	}

	public override void Update() {
		animTime += Time.deltaTime * 3;
		if (animTime > maxIndex) {
			End();
			return;
		}
		var max = Mathf.FloorToInt(animTime);
		LoopTo(max);
		var startPos = path[index].center;
		var targetPos = path[index + 1].center;
		creator.unit.transform.position = Vector3.Lerp(startPos, targetPos, animTime % 1);
	}

	public override bool EventHasEnded() {
		return !animating;
	}

	public override bool End() {
		LoopTo(maxIndex);
		creator.unit.transform.position = end.center;
		animating = false;
		return true;
	}

	protected void LoopTo(int max) {
		while (max > index) {
			index++;
			var last = index == maxIndex;
			var tile = path[index];
			if (last) {
				ExecuteOn(path[index]);
			} else {
				ExecuteOn(path[index]);
				ExecuteOff(path[index]);
			}
			creator.unit.MoveTo(tile);
		}
	}

	protected void ExecuteOn(Tile tile) { Debug.Log("OnMoveOn"); tile.modifiers.Execute(v => v.OnMoveOn(creator.unit)); }
	protected void ExecuteOff(Tile tile) { Debug.Log("OnMoveOff"); tile.modifiers.Execute(v => v.OnMoveOff(creator.unit)); }
}
