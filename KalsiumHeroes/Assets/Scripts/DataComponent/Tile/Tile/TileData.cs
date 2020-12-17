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

	[Tooltip("EdgeModifiers added to the Edges around this Tile")]
	public EdgeModifierData[] edges;

}