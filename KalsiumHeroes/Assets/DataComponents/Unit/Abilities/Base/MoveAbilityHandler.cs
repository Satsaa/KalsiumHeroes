using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAbilityHandler : EventHandler<Events.Ability> {

	public MoveAbility creator;

	[SerializeField, HideInInspector] bool animating;
	[SerializeField, HideInInspector] int index;
	[SerializeField, HideInInspector] List<MasterComponent> pathObjects = new List<MasterComponent>();

	public MoveAbilityHandler(Events.Ability data, MoveAbility creator) : base(data) {
		this.creator = creator;
		var start = Game.grid.tiles[data.unit];
		var end = Game.grid.tiles[data.targets.First()];
		Debug.Log("Handling move ability event!");
		if (end.unit) {
			Debug.LogError("Target Tile is blocked by a unit!");
		} else {
			var rangeMode = creator.abilityData.rangeMode;
			Pathing.CheapestPath(start, end, out var result, Pathers.For(rangeMode), CostCalculators.For(rangeMode));
			animating = true;
			index = -1;
			pathObjects.Clear();

			var cost = result.tiles[result.closest].cost;
			creator.usedMovement += cost;

			// Build list of items to move to
			Tile prev = null;
			foreach (var tile in result.path) {
				if (prev != null) {
					var edge = tile.edges[tile.neighbors.ToList().FindIndex(v => v == prev)];
					pathObjects.Add(edge);
				}
				pathObjects.Add(tile);
				prev = tile;
			}
			var actor = creator.unit.actor;
			actor.Walk(pathObjects.Select(v => v.transform.position));
		}
	}

	public override void Update() {
		var actor = creator.unit.actor;
		if (index + 1 <= actor.moveT) {
			index++;
			if (index >= pathObjects.Count - 1) {
				End();
				return;
			}
			var prev = pathObjects[index];
			var next = pathObjects[index + 1];
			switch (next) {
				case Tile tile:
					ExecuteOver(prev as Edge, pathObjects[index - 1] as Tile);
					break;
				case Edge edge:
					if (index > 0) ExecuteOn(prev as Tile);
					ExecuteOff(prev as Tile);
					break;
			}
		}
	}

	public override bool EventHasEnded() {
		return !animating;
	}

	public override bool End() {
		if (!animating) return true;
		animating = false;
		while (index < pathObjects.Count - 1) {
			Update();
			var actor = creator.unit.actor;
			actor.EndAnimations();
		}
		ExecuteOn(pathObjects.Last() as Tile);
		creator.unit.MoveTo(pathObjects.Last() as Tile, true);
		return true;
	}


	protected void ExecuteOver(Edge edge, Tile source) {
		Debug.Log($"OnMoveOver ({edge.gameObject.name})");
		edge.modifiers.Execute(v => v.OnMoveOver(creator.unit, source));
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
