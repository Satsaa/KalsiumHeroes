using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RockSolidPassive : Passive, IOnChangePosition_Global, IOnDeath_Global, IOnSpawn_Unit {

	public new RockSolidPassiveData data => (RockSolidPassiveData)_data;
	public override Type dataType => typeof(RockSolidPassiveData);

	protected Alterer<int, int> defAlt;
	protected Alterer<int, int> resAlt;

	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);
		defAlt = unit.data.defense.ConfigureValueAlterer(add, this,
			applier: (v, a) => v + a,
			updater: GetIncrease,
			updateEvents: data.rings.Select(v => v.increase.onValueChanged)
		);
		resAlt = unit.data.resistance.ConfigureValueAlterer(add, this,
			applier: (v, a) => v + a,
			updater: GetIncrease,
			updateEvents: data.rings.Select(v => v.increase.onValueChanged)
		);
	}

	private int GetIncrease() {
		var increase = 0;
		for (int i = 0; i < data.rings.Length; i++) {
			var vals = data.rings[i];
			var ring = Game.grid.Ring(unit.tile, i);
			foreach (var tile in ring) {
				foreach (var unit in tile.units.Where(v => data.filter.TargetIsCompatible(unit, v))) {
					increase += vals.increase.value;
				}
			}
		}
		return increase;
	}

	public void OnChangePosition(Unit unit, Tile from, Tile to) => UpdateAlterers();
	public void OnDeath(Unit unit) => UpdateAlterers();
	public void OnSpawn() => UpdateAlterers();

	protected void UpdateAlterers() {
		//!!! No redundancy checks
		defAlt.Update();
		resAlt.Update();
	}
}
