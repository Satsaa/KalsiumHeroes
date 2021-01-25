
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TargetAbility : Ability {

	public new TargetAbilityData source => (TargetAbilityData)_source;
	public new TargetAbilityData data => (TargetAbilityData)_data;
	public override Type dataType => typeof(TargetAbilityData);

	/// <summary> Returns a targeter with onComplete and onCancel callbacks. </summary>
	public virtual Targeter GetTargeter() {
		return new AbilityTargeter(unit, this,
			onComplete: (targeter) => Game.client.PostEvent(new Events.Ability() {
				unit = unit.tile.hex.pos,
				ability = data.identifier,
				targets = new Vector3Int[] { targeter.selections[0].hex.pos },
			})
		);
	}

	/// <summary> Utility method that returns a list of Tiles that satisfy the range values in TargetAbilityData. </summary>
	protected IEnumerable<Tile> GetDefaultRangeTiles() {
		var tile = unit.tile;
		IEnumerable<Tile> res;
		if (data.range.enabled) {
			if (data.rangeMode == RangeMode.Distance) {
				res = Game.grid.Radius(tile, data.range.value);
			} else {
				res = Pathing.GetDistanceField(tile, data.range.value, Pathers.For(data.rangeMode)).tiles.Keys;
			}
		} else {
			res = Game.grid.tiles.Values;
		}

		if (data.requiresVision.value) res = res.Where(h => Game.grid.HasSight(tile, h));

		return res;
	}

}