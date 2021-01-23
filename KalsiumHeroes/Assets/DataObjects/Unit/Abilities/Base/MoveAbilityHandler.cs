﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAbilityHandler : EventHandler<Events.Ability> {

	public MoveAbility creator;

	[SerializeField, HideInInspector] bool animating;
	[SerializeField, HideInInspector] int index;
	[SerializeField, HideInInspector] List<Master> pathObjects = new List<Master>();

	public MoveAbilityHandler(Events.Ability msg, MoveAbility creator) : base(msg) {
		this.creator = creator;
		var start = Game.grid.tiles[msg.unit];
		var end = Game.grid.tiles[msg.targets.First()];
		Debug.Log("Handling move ability event!");
		if (end.unit) {
			Debug.LogError("Target Tile is blocked by a unit!");
		} else {
			var rangeMode = creator.data.rangeMode;
			Pathing.CheapestPath(start, end, out var result, Pathers.For(rangeMode), CostCalculators.For(rangeMode));
			animating = true;
			index = -1;
			pathObjects.Clear();

			var movement = creator.unit.data.movement.value;
			var freeMovement = movement - creator.usedMovement;
			var energyMovement = creator.GetPaidMovement(movement, creator.unit.data.energy.value);

			var cost = result.tiles[result.closest].cost;
			var energyPayment = creator.GetPaidMovementCost(cost, movement);
			creator.unit.data.energy.value -= Mathf.FloorToInt(energyPayment);
			creator.usedMovement += Mathf.Min(freeMovement, cost);

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
		var unit = creator.unit;
		var actor = unit.actor;
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
					ExecuteOver(unit, pathObjects[index - 1] as Tile, prev as Edge, pathObjects[index + 1] as Tile);
					break;
				case Edge edge:
					if (index > 0) ExecuteOn(unit, prev as Tile);
					ExecuteOff(unit, prev as Tile);
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
		ExecuteOn(creator.unit, pathObjects.Last() as Tile);
		creator.unit.MoveTo(pathObjects.Last() as Tile, true);
		return true;
	}


	protected void ExecuteOver(Unit unit, Tile from, Edge edge, Tile to) {
		using (var scope = new OnEvents.Scope()) {
			edge.onEvents.ForEach<IOnMoveOver_Edge>(scope, v => v.OnMoveOver(unit, from, to));
			unit.onEvents.ForEach<IOnMoveOver_Unit>(scope, v => v.OnMoveOver(from, edge, to));
			Game.onEvents.ForEach<IOnMoveOver_Global>(scope, v => v.OnMoveOver(unit, from, edge, to));
		}
		if (unit == null) animating = false;
	}

	protected void ExecuteOn(Unit unit, Tile tile) {
		using (var scope = new OnEvents.Scope()) {
			tile.onEvents.ForEach<IOnMoveOn_Tile>(scope, v => v.OnMoveOn(unit));
			unit.onEvents.ForEach<IOnMoveOn_Unit>(scope, v => v.OnMoveOn(tile));
			Game.onEvents.ForEach<IOnMoveOn_Global>(scope, v => v.OnMoveOn(unit, tile));
		}
		if (unit == null) animating = false;
	}
	protected void ExecuteOff(Unit unit, Tile tile) {
		using (var scope = new OnEvents.Scope()) {
			tile.onEvents.ForEach<IOnMoveOff_Tile>(scope, v => v.OnMoveOff(unit));
			unit.onEvents.ForEach<IOnMoveOff_Unit>(scope, v => v.OnMoveOff(tile));
			Game.onEvents.ForEach<IOnMoveOff_Global>(scope, v => v.OnMoveOff(unit, tile));
		}
		if (unit == null) animating = false;
	}
}