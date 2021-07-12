using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class OpportunistPassive : Passive, IOnMoveOver_Global {

	public new OpportunistPassiveData data => (OpportunistPassiveData)_data;
	public override Type dataType => typeof(OpportunistPassiveData);

	public void OnMoveOver(Modifier reason, Unit unit, Tile from, Edge edge, Tile to) {
		if (reason is MoveAbility) {
			if (unit != this.unit) {
				var oldDistance = Game.grid.Distance(this.unit.tile, from);
				var newDistance = Game.grid.Distance(this.unit.tile, to);
				if (oldDistance <= data.range.current && newDistance > data.range.current && this.unit.team != unit.team) {
					DealDamage(unit, data.damage.current, data.damageType);
					this.unit.data.energy.current.value += data.energyGain.current;
					this.unit.RefreshEnergy();
				}
			}
		}
	}

}
