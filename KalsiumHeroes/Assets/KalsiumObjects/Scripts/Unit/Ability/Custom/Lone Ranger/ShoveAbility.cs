using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(ShoveAbility), menuName = "KalsiumHeroes/Ability/" + nameof(ShoveAbility))]
public class ShoveAbility : UnitTargetAbility {

	[Tooltip("This is the Modifier given to the targeted Unit")]
	public UnitModifier modifier;

	[Tooltip("The maximum amount of tiles the target is shoved")]
	public Attribute<int> shoveDist = new(4);


	public override EventHandler<GameEvents.Ability> CreateHandler(GameEvents.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var target = Game.grid.tiles[msg.targets[0]].units[msg.targetIndexes[0]];
			Create(target, modifier);
			Shove(target);
		});
	}

	void Shove(Unit target) {
		var dir = unit.tile.GetDir(target.tile);
		for (int i = 0; i < shoveDist.current; i++) {
			if (target.CanMoveInDir(dir, out Tile next)) {
				ExecuteMoveOff(target, target.tile);
				ExecuteMoveOver(target, target.tile, next);
				target.SetTile(next, true);
				ExecuteMoveOn(target, next);
			} else {
				break;
			}
		}
	}
}
