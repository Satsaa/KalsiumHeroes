using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Mathf;

[CreateAssetMenu(fileName = nameof(TileData), menuName = "DataSources/" + nameof(TileData))]
public class TileData : MasterComponentData {

	[Header("Tile Data")]
	[Tooltip("Is this tile considered passable?")]
	public Attribute<bool> passable = new Attribute<bool>(true);

	[Tooltip("Can this Tile be seen through?")]
	public Attribute<bool> transparent = new Attribute<bool>(true);

	[Tooltip("Cost of movement to this Tile.")]
	public Attribute<float> moveCost = new Attribute<float>(1);

	[Tooltip("More appealing Tiles are preferred when pathfinding.")]
	public Attribute<float> appeal = new Attribute<float>(0);

	[Tooltip("EdgeModifiers added to the Edges around this Tile. Please leave at length 6.")]
	public TileEdgeModifierCollection edgeModifiers;

}

[Serializable]
public class TileEdgeModifierCollection {

	public EdgeModifierData[] this[int index] {
		get => index switch {
			0 => this.right,
			1 => this.downRight,
			2 => this.downLeft,
			3 => this.left,
			4 => this.upLeft,
			5 => this.upRigth,
			_ => throw new ArgumentOutOfRangeException(nameof(index)),
		};
	}

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