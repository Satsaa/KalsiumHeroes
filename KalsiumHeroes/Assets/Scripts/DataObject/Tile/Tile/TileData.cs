using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Mathf;

[CreateAssetMenu(fileName = nameof(TileData), menuName = "DataSources/" + nameof(TileData))]
public class TileData : MasterData {

	public override Type createTypeConstraint => typeof(Tile);

	[Tooltip("Is this tile considered passable?")]
	public Passable passable;

	[Tooltip("Can this Tile be seen through?")]
	public Transparent transparent;

	[Tooltip("Cost of movement to this Tile.")]
	public MoveCost moveCost;

	[Tooltip("More appealing Tiles are preferred when pathfinding.")]
	public Appeal appeal;

	[Tooltip("EdgeModifiers added to the Edges around this Tile.")]
	public TileEdgeModifierCollection edgeModifiers;

	[Serializable]
	public class Passable : Attribute<bool> {
		Passable() : base(true) { }
		public override string identifier => "Attribute_Tile_Passable";
		public override string TooltipText(IAttribute source) {
			if (!current) return DefaultTooltip(source);
			return null;
		}
	}

	[Serializable]
	public class Transparent : Attribute<bool> {
		Transparent() : base(true) { }
		public override string identifier => "Attribute_Tile_Transparent";
		public override string TooltipText(IAttribute source) {
			if (!current) return DefaultTooltip(source);
			return null;
		}
	}

	[Serializable]
	public class MoveCost : Attribute<float> {
		MoveCost() : base(1) { }
		public override string identifier => "Attribute_Tile_MoveCost";
		public override string TooltipText(IAttribute source) {
			if (current != 1) return DefaultTooltip(source);
			return null;
		}
	}

	[Serializable]
	public class Appeal : Attribute<float> {
		public override string identifier => "Attribute_Tile_Appeal";
		public override string TooltipText(IAttribute source) {
			if (current != 0) return DefaultTooltip(source);
			return null;
		}
	}
}

[Serializable]
public class TileEdgeModifierCollection {

	public EdgeModifierData[] this[int index] => index switch {
		0 => right,
		1 => downRight,
		2 => downLeft,
		3 => left,
		4 => upLeft,
		5 => upRigth,
		_ => throw new ArgumentOutOfRangeException(nameof(index)),
	};

	[Tooltip("EdgeModifiers added to the " + nameof(TileDir.Right) + " Edge.")]
	public EdgeModifierData[] right;
	[Tooltip("EdgeModifiers added to the " + nameof(TileDir.DownRight) + " Edge.")]
	public EdgeModifierData[] downRight;
	[Tooltip("EdgeModifiers added to the " + nameof(TileDir.DownLeft) + " Edge.")]
	public EdgeModifierData[] downLeft;
	[Tooltip("EdgeModifiers added to the " + nameof(TileDir.Left) + " Edge.")]
	public EdgeModifierData[] left;
	[Tooltip("EdgeModifiers added to the " + nameof(TileDir.UpLeft) + " Edge.")]
	public EdgeModifierData[] upLeft;
	[Tooltip("EdgeModifiers added to the " + nameof(TileDir.UpRight) + " Edge.")]
	public EdgeModifierData[] upRigth;
}