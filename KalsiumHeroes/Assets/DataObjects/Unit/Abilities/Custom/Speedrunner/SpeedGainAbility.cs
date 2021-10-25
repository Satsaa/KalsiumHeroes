using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpeedGainAbility : NoTargetAbility {

	[Tooltip("Radius of checked for other units")]
	public Attribute<int> radius = new(2);

	[Tooltip("This is the actual speed gain modifier gained from using the spell")]
	public SpeedGainStatus speedGainStatus;

	[Tooltip("Gain speed from units matching any of these unit target types.")]
	public UnitTargetType teamType;


	public override EventHandler<GameEvents.Ability> CreateHandler(GameEvents.Ability msg) {
		return new InstantAbilityHandler(msg, this, (Ability) => {
			var unitsFound = 0;
			var aoe = GetAffectedArea();
			foreach (var tile in aoe) {
				foreach (var unit in tile.units) {
					if (teamType.TargetIsCompatible(this.unit, unit)) unitsFound++;
				}
			}
			Create(master, speedGainStatus, v => v.Init(unitsFound));
		});
	}

	public override IEnumerable<Tile> GetAffectedArea() {
		return Game.grid.Radius(unit.tile, radius.current);
	}
}
