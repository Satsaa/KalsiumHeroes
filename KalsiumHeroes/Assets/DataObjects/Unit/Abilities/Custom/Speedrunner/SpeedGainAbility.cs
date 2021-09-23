using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpeedGainAbility : NoTargetAbility {

	new public SpeedGainAbilityData data => (SpeedGainAbilityData)_data;
	public override Type dataType => typeof(SpeedGainAbilityData);

	public override EventHandler<GameEvents.Ability> CreateHandler(GameEvents.Ability msg) {
		return new InstantAbilityHandler(msg, this, (Ability) => {
			var unitsFound = 0;
			var aoe = GetAffectedArea();
			foreach (var tile in aoe) {
				foreach (var unit in tile.units) {
					if (data.teamType.TargetIsCompatible(this.unit, unit)) unitsFound++;
				}
			}
			SpeedGainStatus.Create(master, data.speedGainModifier, v => ((SpeedGainStatus)v).Init(unitsFound));
		});
	}

	public override IEnumerable<Tile> GetAffectedArea() {
		return Game.grid.Radius(unit.tile, data.radius.current);
	}
}
