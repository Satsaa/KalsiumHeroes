using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoveAbility : Ability {
	public ShoveAbilityData shoveAbilityData => (ShoveAbilityData)data;
	public override Type dataType => typeof(ShoveAbilityData);

	[HideInInspector]
	public Tile.Dir direction;
	[HideInInspector]
	bool dontShove = false;

	public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability data) {
		return new InstantAbilityHandler(data, this, (ability) => {
			dontShove = false;
			var target = Game.grid.tiles[data.target];
			var aoe = GetAffectedArea(target);
			foreach (var tile in aoe) {
				if (tile.unit) {
					tile.unit.gameObject.AddEntityComponent(shoveAbilityData.rootModifier);
					tile.unit.MovePosition(GetTargetTile(tile));
				}
			}
		});
	}

	Tile GetTargetTile(Tile tile) {
		direction = CheckDirection(tile);
		if (!dontShove) {
			if (direction == Tile.Dir.DownRight) {
				while (tile.downRight != null && !tile.downRight.unit && !tile.downRight.blocked) {
					tile = tile.downRight;
				}
			}
			if (direction == Tile.Dir.DownLeft) {
				while (tile.downLeft != null && !tile.downLeft.unit && !tile.downLeft.blocked) {
					tile = tile.downLeft;
				}
			}
			if (direction == Tile.Dir.Left) {
				while (tile.left != null && !tile.left.unit && !tile.left.blocked) {
					tile = tile.left;
				}
			}
			if (direction == Tile.Dir.UpLeft) {
				while (tile.upLeft != null && !tile.upLeft.unit && !tile.upLeft.blocked) {
					tile = tile.upLeft;
				}
			}
			if (direction == Tile.Dir.UpRight) {
				while (tile.upRight != null && !tile.upRight.unit && !tile.upRight.blocked) {
					tile = tile.upRight;
				}
			}
			if (direction == Tile.Dir.Right) {
				while (tile.right != null && !tile.right.unit && !tile.right.blocked) {
					tile = tile.right;
				}
			}
		}
		return tile;
	}


	Tile.Dir CheckDirection(Tile tile) {
		var u = unit;
		if (tile.upLeft.unit && tile.upLeft.unit == u) {
			return Tile.Dir.DownRight;
		}
		if (tile.upRight.unit && tile.upRight.unit == u) {
			return Tile.Dir.DownLeft;
		}
		if (tile.right.unit && tile.right.unit == u) {
			return Tile.Dir.Left;
		}
		if (tile.downRight.unit && tile.downRight.unit == u) {
			return Tile.Dir.UpLeft;
		}
		if (tile.downLeft.unit && tile.downLeft.unit == u) {
			return Tile.Dir.UpRight;
		}
		if (tile.left.unit && tile.left.unit == u) {
			return Tile.Dir.Right;
		} else {
			Debug.LogError("Unit was not in melee range! Something has gone wrong!");
			dontShove = true;
			return Tile.Dir.Right;
		}
	}
}
