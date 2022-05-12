
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TargetAbility : Ability {

	[Tooltip("Cast range of the ability.")]
	public Range range;

	[Tooltip("How the range is determined.")]
	public RangeMode rangeMode;

	[Tooltip("Only directly visible Tiles are valid in range?")]
	public RequiresVision requiresVision;

	[Serializable]
	public class Range : ToggleAttribute<int> {
		Range() : base(1) { }
		public override string identifier => "Attribute_TargetAbility_Range";
		public override string Format(bool isSource) => !enabled ? Lang.GetStr("Infinite") : base.Format(isSource);
	}

	[Serializable]
	public class RequiresVision : Attribute<bool> {
		RequiresVision() : base(false) { }
		public override string identifier => "Attribute_TargetAbility_RequiresVision";
		public override string TooltipText(IAttribute source) => current ? null : DefaultTooltip(source);
	}


	/// <summary> Returns a targeter with onComplete and onCancel callbacks. </summary>
	public virtual Targeter GetTargeter() {
		return new AbilityTargeter(unit, this,
			onComplete: (targeter) => App.client.Post(
				new GameEvents.Ability() {
					casterTile = unit.tile.hex.pos,
					casterIndex = unit.tile.units.IndexOf(unit),
					abilityIndex = unit.modifiers.IndexOf<Ability>(this),
					targets = targeter.selections.Select(v => v.hex.pos).ToArray(),
					targetIndexes = targeter.selections.Select(v => 0).ToArray() // Todo unit target index
				}
			)
		);
	}

	/// <summary> Utility method that returns a list of Tiles that satisfy the range values in TargetAbilityData. </summary>
	protected IEnumerable<Tile> GetDefaultRangeTiles() {
		var tile = unit.tile;
		IEnumerable<Tile> res =
			range.enabled
				? rangeMode == RangeMode.Distance
					? Game.grid.Radius(tile, range.current)
					: Pathing.GetDistanceField(tile, range.current, Pathers.For(rangeMode)).tiles.Keys
				: Game.grid.tiles.Values;
		if (requiresVision.current) res = res.Where(h => Game.grid.HasSight(tile, h));

		return res;
	}

}