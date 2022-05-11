
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using HexGrid;
using Muc.Extensions;
using Muc.Numerics;
using Muc.Collections;

[ExecuteAlways]
public class Tile : Master<Tile, ITileHook, Actor, TileModifier>, IOnDeath_Tile {

	[Tooltip("Is this tile considered passable?")]
	public Passable passable;

	[Tooltip("Can this Tile be seen through?")]
	public Transparent transparent;

	[Tooltip("Cost of movement to this Tile.")]
	public MoveCost moveCost;

	[Tooltip("More appealing Tiles are preferred when pathfinding.")]
	public Appeal appeal;

	[Tooltip("EdgeModifiers added to the Edges around this Tile.")]
	public TileEdgeModifierCollection baseEdgeModifiers;

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

	[Serializable]
	public class TileEdgeModifierCollection {

		public EdgeModifier[] this[int index] => index switch {
			0 => right,
			1 => downRight,
			2 => downLeft,
			3 => left,
			4 => upLeft,
			5 => upRigth,
			_ => throw new ArgumentOutOfRangeException(nameof(index)),
		};

		[Tooltip("EdgeModifiers added to the " + nameof(TileDir.Right) + " Edge.")]
		public EdgeModifier[] right;
		[Tooltip("EdgeModifiers added to the " + nameof(TileDir.DownRight) + " Edge.")]
		public EdgeModifier[] downRight;
		[Tooltip("EdgeModifiers added to the " + nameof(TileDir.DownLeft) + " Edge.")]
		public EdgeModifier[] downLeft;
		[Tooltip("EdgeModifiers added to the " + nameof(TileDir.Left) + " Edge.")]
		public EdgeModifier[] left;
		[Tooltip("EdgeModifiers added to the " + nameof(TileDir.UpLeft) + " Edge.")]
		public EdgeModifier[] upLeft;
		[Tooltip("EdgeModifiers added to the " + nameof(TileDir.UpRight) + " Edge.")]
		public EdgeModifier[] upRigth;
	}


	public static implicit operator Hex(Tile v) => v.hex;

	public bool hasUnits => units.Count > 0;
	public SafeList<Unit> units;

	[field: SerializeField] List<GraveUnit> _graveyard = new();
	public IReadOnlyList<GraveUnit> graveyard => _graveyard;

	[field: SerializeField] public Highlighter highlighter { get; private set; } = new();

	[field: SerializeField] public Hex hex { get; private set; }

	[field: SerializeField] public Vector3 center { get; private set; }
	[field: SerializeField] public Vector3[] corners { get; private set; }

	[field: SerializeField] public Tile[] neighbors { get; private set; } = new Tile[6];
	[field: SerializeField] public Edge[] edges { get; private set; } = new Edge[6];

	/// <summary> Creates a Tile based on the given source and hex. </summary>
	public static T Create<T>(T source, Hex hex) where T : Tile {
		if (Game.grid.tiles.ContainsKey(hex.pos)) throw new InvalidOperationException("There is already a Tile at the Hex.");
		return Create(source, v => {
			v.hex = hex;
			Game.grid.tiles.Add(hex.pos, v);
			var pt = Layout.HexToPoint(hex);
			v.center = new Vector3(pt.x, source.baseActor.transform.position.y, pt.y);
			v.corners = Layout.Corners(hex).Select(v => new Vector3(v.x, 0, v.y)).ToArray();
		});
	}

	protected override void OnCreate() {
		base.OnCreate();
		// Find neighbors
		for (int i = 0; i < neighbors.Length; i++) {
			if (Game.grid.tiles.TryGetValue(hex.GetNeighbor(i).pos, out var nbr)) {
				SetNeighbor(i, nbr);
			}
		}
		// Find or create edges
		for (int i = 0; i < neighbors.Length; i++) {
			var nbr = neighbors[i];
			if (nbr) {
				var opposite = new CircularInt(i - 3, 6);
				SetEdge(i, nbr.GetEdge(opposite), false);
			} else {
				Edge.Create(Game.grid.defaultEdge, this, (TileDir)i);
			}
		}
		// Create default edge modifiers
		for (int i = 0; i < neighbors.Length; i++) {
			var edge = edges[i];
			Debug.Assert(edge); // Should be set for all direction
			foreach (var edgeSource in baseEdgeModifiers[i]) {
				EdgeModifier.Create(edge, edgeSource, this);
			}
		}
	}

	protected override void OnRemove() {
		for (int i = 0; i < neighbors.Length; i++) {
			var nbr = neighbors[i];
			var edge = edges[i];
			if (nbr) {
				nbr.SetNeighbor(new CircularInt(i - 3, 6), null);
			}
			if (edge) {
				if (nbr == null) {
					edge.Remove();
				}
				edge.RemoveModifiersByContext(this);
			}
		}
		Game.grid.tiles.Remove(hex.pos);
		ObjectUtil.Destroy(gameObject);
		base.OnRemove();
	}

	protected override void OnShow() {
		base.OnShow();
		gameObject.transform.position = Layout.HexToPoint(hex).xxy().SetY(actor.transform.position.y);
		gameObject.transform.parent = Game.game.transform;
		gameObject.name = $"Tile ({hex.x}, {hex.y})";
		highlighter.OnShow(gameObject);
		transform.position = center.SetY(transform.position.y);
	}

	protected override void OnHide() {
		highlighter.OnHide();
		base.OnHide();
	}

	void IOnDeath_Tile.OnDeath(Unit unit) {
		_graveyard.Add(new(unit));
		units.Remove(unit);
	}

	public IEnumerable<(Tile tile, Edge edge)> NeighborsWithEdges() {
		for (int i = 0; i < neighbors.Length; i++) {
			var neighbor = neighbors[i];
			if (neighbor == null) continue;
			var edge = edges[i];
			yield return (neighbor, edge);
		}
	}

	public TileDir GetDir(Tile neighbor) {
		if (neighbor is null) throw new ArgumentNullException(nameof(neighbor));
		var res = Array.FindIndex(neighbors, v => v == neighbor);
		if (res == -1) throw new ArgumentOutOfRangeException("The tile must be a direct neighbor.", nameof(neighbor));
		return (TileDir)res;
	}

	public IEnumerable<Tile> Neighbors() => neighbors.Where(v => v != null);

	public Tile GetNeighbor(TileDir direction) => neighbors[(int)direction];
	public Tile GetNeighbor(int direction) => neighbors[direction];

	public void SetNeighbor(TileDir direction, Tile tile, bool propagate = true) => SetNeighbor((int)direction, tile, propagate);
	public void SetNeighbor(int direction, Tile tile, bool propagate = true) {
		neighbors[direction] = tile;
		if (!propagate || tile == null) return;
		tile.SetNeighbor(new CircularInt(direction - 3, 6), this, false);
	}

	public IEnumerable<Edge> Edges() => edges.Where(v => v != null);

	public Edge GetEdge(TileDir direction) => edges[(int)direction];
	public Edge GetEdge(int direction) => edges[direction];

	public void SetEdge(TileDir direction, Edge edge, bool propagate = true) => SetEdge((int)direction, edge, propagate);
	public void SetEdge(int direction, Edge edge, bool propagate = true) {
		edges[direction] = edge;
		if (!propagate) return;
		if (edge != null) {
			edge.hex1 = this;
			edge.hex2 = hex.GetNeighbor(direction);
		}
		var neighbor = GetNeighbor(direction);
		if (neighbor) neighbor.SetEdge(new CircularInt(direction - 3, 6), edge, false);
	}

	public bool PathTest(int dir, UnitPather pather, Unit unit) => PathTest((TileDir)dir, pather, unit);
	public bool PathTest(TileDir dir, UnitPather pather, Unit unit) {
		var from = this;
		var edge = GetEdge(dir);
		var to = GetNeighbor(dir);
		if (to == null) return false;
		return pather(unit, from, edge, to);
	}
	public bool PathTest(int dir, Pather pather) => PathTest((TileDir)dir, pather);
	public bool PathTest(TileDir dir, Pather pather) {
		var from = this;
		var edge = GetEdge(dir);
		var to = GetNeighbor(dir);
		if (to == null) return false;
		return pather(from, edge, to);
	}

	public Edge EdgeBetween(Tile tile2) {
		if (tile2 is null) throw new ArgumentNullException(nameof(tile2));
		var dir = GetDir(tile2);
		var edge = GetEdge(dir);
		return edge;
	}

}
