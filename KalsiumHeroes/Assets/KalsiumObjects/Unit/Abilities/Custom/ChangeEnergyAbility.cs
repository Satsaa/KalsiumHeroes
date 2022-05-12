using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(ChangeEnergyAbility), menuName = "KalsiumHeroes/Ability/" + nameof(ChangeEnergyAbility))]
public class ChangeEnergyAbility : UnitTargetAbility {

	public Attribute<int> energyChange;


	public override EventHandler<GameEvents.Ability> CreateHandler(GameEvents.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var target = Game.grid.tiles[msg.targets[0]].units[msg.targetIndexes[0]];
			var aoe = GetAffectedArea(target);
			foreach (var tile in aoe) {
				foreach (var unit in tile.units) {
					unit.energy.current.value += energyChange.current;
					unit.RefreshEnergy();
				}
			}
		});
	}

}
