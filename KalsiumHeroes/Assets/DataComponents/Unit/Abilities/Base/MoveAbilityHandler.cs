using System;
using System.Linq;
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

	[SerializeField, HideInInspector] int eventIndex;
	int maxIndex => (path.Length - 1) * 2;
	int tileIndex => eventIndex / 2;

	public MoveAbilityHandler(Events.Ability data, MoveAbility creator) : base(data) {
		this.creator = creator;
		start = Game.grid.tiles[data.unit];
		end = Game.grid.tiles[data.targets.First()];
		Debug.Log("Handling move ability event!");
		if (end.unit) {
			Debug.LogError("Target Tile is blocked by a unit!");
		} else {
			ExecuteOff(start);
			var rangeMode = creator.abilityData.rangeMode;
			Pathing.CheapestPath(start, end, out var result, Pathers.For(rangeMode), CostCalculators.For(rangeMode));
			path = result.path;
			var cost = result.tiles[result.closest].cost;
			creator.usedMovement += cost;
			animating = true;
			animTime = 0;
		}
	}

	public override void Update() {
		animTime += Time.deltaTime * 3;
		if (animTime > maxIndex / 2) {
			End();
			return;
		}
		var newIndex = Mathf.FloorToInt(animTime * 2);
		LoopTo(newIndex);
		var startPos = path[tileIndex].center;
		var targetPos = path[tileIndex + 1].center;
		creator.unit.transform.position = Vector3.Lerp(startPos, targetPos, animTime % 1);
	}

	public override bool EventHasEnded() {
		return !animating;
	}

	public override bool End() {
		LoopTo(maxIndex);
		creator.unit.MoveTo(end, true);
		animating = false;
		return true;
	}

	protected void LoopTo(int max) {
		while (max > eventIndex) {
			eventIndex++;
			var tile = path[tileIndex];
			if (eventIndex % 2 == 1) {
				var next = path[tileIndex + 1];
				ExecuteOver(tile, next);
			} else {
				if (eventIndex == maxIndex) {
					ExecuteOn(path[tileIndex]);
				} else {
					ExecuteOn(path[tileIndex]);
					ExecuteOff(path[tileIndex]);
				}
			}
		}
	}

	protected void ExecuteOver(Tile current, Tile next) {
		Debug.Log($"OnMoveOver ({current.gameObject.name} -> {next.gameObject.name})");
		var edge = current.edges[current.neighbors.ToList().FindIndex(v => v == next)];
		edge.modifiers.Execute(v => v.OnMoveOver(creator.unit, current));
		Game.InvokeOnAfterEvent();
	}

	protected void ExecuteOn(Tile tile) {
		Debug.Log($"OnMoveOn ({tile.gameObject.name})");
		tile.modifiers.Execute(v => v.OnMoveOn(creator.unit));
		Game.InvokeOnAfterEvent();
	}
	protected void ExecuteOff(Tile tile) {
		Debug.Log($"OnMoveOff ({tile.gameObject.name})");
		tile.modifiers.Execute(v => v.OnMoveOff(creator.unit));
		Game.InvokeOnAfterEvent();
	}
}
