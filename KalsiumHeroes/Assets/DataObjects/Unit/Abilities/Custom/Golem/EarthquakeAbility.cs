using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthquakeAbility : Ability {

	public new EarthquakeAbilityData data => (EarthquakeAbilityData)_data;
	public override Type dataType => typeof(EarthquakeAbilityData);

	private static List<Unit> unitsFound = new List<Unit>();

	public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			GetAffectedUnits();
		});
	}

	void GetAffectedUnits() {
		var h = unit.tile;
		var radius = Game.grid.Ring(h, 1);
		foreach (var tile in radius) {
			if (tile.unit && tile.unit != unit) {
				unitsFound.Add(tile.unit);
				tile.unit.DealCalculatedDamage(this, data.damage1.value, data.damageType);
				Modifier.Create(tile.unit, data.rootModifier);
			}
		}
		radius = Game.grid.Ring(h, 2);
		foreach (var tile in radius) {
			if (tile.unit && tile.unit != unit && !unitsFound.Contains(tile.unit)) {
				unitsFound.Add(tile.unit);
				tile.unit.DealCalculatedDamage(this, data.damage2.value, data.damageType);
				Modifier.Create(tile.unit, data.slowModifier2);
			}
		}
		radius = Game.grid.Ring(h, 3);
		foreach (var tile in radius) {
			if (tile.unit && tile.unit != unit && !unitsFound.Contains(tile.unit)) {
				tile.unit.DealCalculatedDamage(this, data.damage3.value, data.damageType);
				Modifier.Create(tile.unit, data.slowModifier3);
			}
		}
		unitsFound.Clear();
	}
}
