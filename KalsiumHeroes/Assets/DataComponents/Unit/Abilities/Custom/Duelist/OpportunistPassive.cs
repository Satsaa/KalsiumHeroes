using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class OpportunistPassive : Passive, IOnMoveOver_Global {

	public new OpportunistPassiveData data => (OpportunistPassiveData)_data;
	public override Type dataType => typeof(OpportunistPassiveData);

	public void OnMoveOver(Unit unit, Tile from, Edge edge, Tile to) {
		if (unit != this.unit) {
			var oldDistance = Game.grid.Distance(this.unit.tile, from);
			var newDistance = Game.grid.Distance(this.unit.tile, to);
			if (oldDistance <= data.range.value && newDistance > data.range.value && this.unit.team != unit.team) {
				DealDamage(unit, data.damage.value, data.damageType);
				this.unit.data.energy.value += data.energyGain.value;
				this.unit.RefreshEnergy();
			}
		}
	}

}
