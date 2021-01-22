using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RockSolidPassive : Passive {

	public new RockSolidPassiveData data => (RockSolidPassiveData)_data;
	public override Type dataType => typeof(RockSolidPassiveData);

	private static List<Unit> unitsFound = new List<Unit>();
	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);
		unit.data.defense.ConfigureAlterer(add, v => v + GetUnitsInArea());
		unit.data.resistance.ConfigureAlterer(add, v => v + GetUnitsInArea());
	}

	int GetUnitsInArea() {
		var i = 0;
		var h = unit.tile;
		var radius = Game.grid.Ring(h, 1);
		foreach (var tile in radius) {
			if (tile.unit && tile.unit != unit) {
				unitsFound.Add(tile.unit);
				i += data.increase1.value;
			}
		}
		radius = Game.grid.Ring(h, 2);
		foreach (var tile in radius) {
			if (tile.unit && tile.unit != unit && !unitsFound.Contains(tile.unit)) {
				unitsFound.Add(tile.unit);
				i += data.increase2.value;
			}
		}
		radius = Game.grid.Ring(h, 3);
		foreach (var tile in radius) {
			if (tile.unit && tile.unit != unit && !unitsFound.Contains(tile.unit)) {
				i += data.increase3.value;
			}
		}
		unitsFound.Clear();
		return i;
	}
}
