using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChangeEnergyAbility : UnitTargetAbility {

	new public ChangeEnergyAbilityData data => (ChangeEnergyAbilityData)_data;
	public override Type dataType => typeof(ChangeEnergyAbilityData);

	public override EventHandler<GameEvents.Ability> CreateHandler(GameEvents.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var target = Game.grid.tiles[msg.targets[0]].units[msg.targetIndexes[0]];
			var aoe = GetAffectedArea(target);
			foreach (var tile in aoe) {
				foreach (var unit in tile.units) {
					unit.data.energy.current.value += data.energyChange.current;
					unit.RefreshEnergy();
				}
			}
		});
	}

}
