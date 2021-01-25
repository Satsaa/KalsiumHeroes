using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChangeEnergyAbility : UnitTargetAbility {

	public new ChangeEnergyAbilityData data => (ChangeEnergyAbilityData)_data;
	public override Type dataType => typeof(ChangeEnergyAbilityData);

	public override EventHandler<Events.Ability> CreateHandler(Events.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var target = Game.grid.tiles[msg.targets.First()].unit;
			var aoe = GetAffectedArea(target);
			foreach (var tile in aoe) {
				if (tile.unit) {
					tile.unit.data.energy.value += data.energyChange.value;
					tile.unit.RefreshEnergy();
				}
			}
		});
	}

}
