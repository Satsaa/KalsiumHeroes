using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChangeEnergyAbility : UnitTargetAbility {

	public new ChangeEnergyAbilityData data => (ChangeEnergyAbilityData)_data;
	public override Type dataType => typeof(ChangeEnergyAbilityData);

	public override EventHandler<GameEvents.Ability> CreateHandler(GameEvents.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var target = Game.grid.tiles[msg.targets.First()].units[msg.targetIndexes.First()];
			var aoe = GetAffectedArea(target);
			foreach (var tile in aoe) {
				foreach (var unit in tile.units) {
					unit.data.energy.value.value += data.energyChange.value;
					unit.RefreshEnergy();
				}
			}
		});
	}

}
