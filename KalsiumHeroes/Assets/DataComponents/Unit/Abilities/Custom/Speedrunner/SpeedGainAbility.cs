using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpeedGainAbility : Ability {

	public new SpeedGainAbilityData data => (SpeedGainAbilityData)base.data;
	public override Type dataType => typeof(SpeedGainAbilityData);

	public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability msg) {
		return new InstantAbilityHandler(msg, this, (Ability) => {
			var unitsFound = 0;
			var target = Game.grid.tiles[msg.targets.First()];
			var aoe = GetAffectedArea(target);
			foreach (var tile in aoe) {
				if (tile.unit && tile.unit != unit) unitsFound++;
			}
			unit.gameObject.AddDataComponent<SpeedGainStatus>(this.data.speedGainModifier, v => v.unitsFound = unitsFound);
		});
	}
}
