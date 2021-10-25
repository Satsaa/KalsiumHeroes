using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class OpportunistPassive : Passive, IOnMoveOver_Game {

	public Attribute<float> damage;
	public DamageType damageType;

	public Attribute<int> range;

	public Attribute<int> energyGain;


	public void OnMoveOver(Modifier reason, Unit unit, Tile from, Edge edge, Tile to) {
		if (reason is MoveAbility) {
			if (unit != this.unit) {
				var oldDistance = Game.grid.Distance(this.unit.tile, from);
				var newDistance = Game.grid.Distance(this.unit.tile, to);
				if (oldDistance <= range.current && newDistance > range.current && this.unit.team != unit.team) {
					DealDamage(unit, damage.current, damageType);
					this.unit.energy.current.value += energyGain.current;
					this.unit.RefreshEnergy();
				}
			}
		}
	}

}
