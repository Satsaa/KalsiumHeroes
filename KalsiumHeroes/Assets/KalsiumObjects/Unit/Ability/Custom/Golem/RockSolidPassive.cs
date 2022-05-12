using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(RockSolidPassive), menuName = "KalsiumHeroes/Passive/" + nameof(RockSolidPassive))]
public class RockSolidPassive : Passive, IOnChangePosition_Game, IOnDeath_Game, IOnSpawn_Unit {

	[Tooltip("Matching units give resistances.")]
	public UnitTargetType filter;

	[Tooltip("Rings around the caster and their values")]
	public Ring[] rings;

	[Serializable]
	public class Ring {
		[Tooltip("Added Defense and Resistance from units in this ring.")]
		public Attribute<int> increase;
	}


	protected Alterer<int, int> defAlt;
	protected Alterer<int, int> resAlt;

	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);
		defAlt = unit.defense.current.ConfigureAlterer(add, this,
			applier: (v, a) => v + a,
			updater: GetIncrease,
			updateEvents: rings.Select(v => v.increase.current.onChanged)
		);
		resAlt = unit.resistance.current.ConfigureAlterer(add, this,
			applier: (v, a) => v + a,
			updater: GetIncrease,
			updateEvents: rings.Select(v => v.increase.current.onChanged)
		);
	}

	private int GetIncrease() {
		var increase = 0;
		for (int i = 0; i < rings.Length; i++) {
			var vals = rings[i];
			var ring = Game.grid.Ring(unit.tile, i);
			foreach (var tile in ring) {
				foreach (var unit in tile.units.Where(v => filter.TargetIsCompatible(unit, v))) {
					increase += vals.increase.current;
				}
			}
		}
		return increase;
	}

	public void OnChangePosition(Unit unit, Tile from, Tile to) => UpdateAlterers();
	public void OnDeath(Unit unit) => UpdateAlterers();
	public void OnSpawn() => UpdateAlterers();

	protected void UpdateAlterers() {
		defAlt.Update();
		resAlt.Update();
	}
}
