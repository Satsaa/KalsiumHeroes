
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(EarthquakeAbility), menuName = "KalsiumHeroes/Ability/" + nameof(EarthquakeAbility))]
public class EarthquakeAbility : NoTargetAbility {


	[Tooltip("Matching units get affected by this ability.")]
	public UnitTargetType affected;

	[Tooltip("Values per ring around caster.")]
	public RingValues[] rings;

	[Serializable]
	public class RingValues {
		public DamageType damageType;
		public Attribute<float> damage;
		public UnitModifier modifier;
	}


	private static List<Unit> unitsFound = new();

	public override EventHandler<GameEvents.Ability> CreateHandler(GameEvents.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => GetAffectedUnits());
	}

	void GetAffectedUnits() {
		for (int i = 0; i < rings.Length; i++) {
			var vals = rings[i];
			if (vals.damage.current == 0 && vals.modifier == null) continue;
			var ring = Game.grid.Ring(unit.tile, i);
			foreach (var tile in ring) {
				foreach (var unit in tile.units.Where(v => affected.TargetIsCompatible(unit, v))) {
					unitsFound.Add(unit);
					unit.DealCalculatedDamage(this, vals.damage.current, vals.damageType);
					Create(unit, vals.modifier);
				}
			}
		}
		unitsFound.Clear();
	}
}
