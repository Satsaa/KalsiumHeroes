using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthquakeAbility : NoTargetAbility {

	public new EarthquakeAbilityData data => (EarthquakeAbilityData)_data;
	public override Type dataType => typeof(EarthquakeAbilityData);

	private static List<Unit> unitsFound = new List<Unit>();

	public override EventHandler<Events.Ability> CreateHandler(Events.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			GetAffectedUnits();
		});
	}

	void GetAffectedUnits() {
		for (int i = 0; i < data.rings.Length; i++) {
			var vals = data.rings[i];
			if (vals.damage.value == 0 && vals.modifier == null) continue;
			var ring = Game.grid.Ring(unit.tile, i);
			foreach (var tile in ring) {
				foreach (var unit in tile.units.Where(v => data.affected.TargetIsCompatible(unit, v))) {
					unitsFound.Add(unit);
					unit.DealCalculatedDamage(this, vals.damage.value, data.damageType);
					Modifier.Create(unit, vals.modifier);
				}
			}
		}
		unitsFound.Clear();
	}
}
