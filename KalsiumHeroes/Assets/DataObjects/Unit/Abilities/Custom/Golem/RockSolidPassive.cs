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
		unit.data.defense.ConfigureAlterer(add, v => v + GetIncrease());
		unit.data.resistance.ConfigureAlterer(add, v => v + GetIncrease());
	}

	private int GetIncrease() {
		var increase = 0;
		for (int i = 0; i < data.rings.Length; i++) {
			var vals = data.rings[i];
			if (vals.increase.value == 0) continue;
			var ring = Game.grid.Ring(unit.tile, i);
			foreach (var tile in ring) {
				foreach (var unit in tile.units.Where(v => data.filter.TargetIsCompatible(unit, v))) {
					increase += vals.increase.value;
				}
			}
		}
		return increase;
	}
}
