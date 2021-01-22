
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using HexGrid;
using Muc.Extensions;
using Muc.Numerics;

[ExecuteAlways]
public class Tile : Master<TileModifier, ITileOnEvent> {

	public static implicit operator Hex(Tile v) => v.hex;

	public new TileData source => (TileData)_source;
	public new TileData data => (TileData)_data;
	public override Type dataType => typeof(TileData);
	public static Type modifierDataType => typeof(TileModifierData);


	public Unit unit;

	public List<GraveUnit> graveyard;
	[field: SerializeField] public Highlighter highlighter;

	[field: SerializeField] public Hex hex { get; private set; }

	[field: SerializeField] public Vector3 center { get; private set; }
	[field: SerializeField] public Vector3[] corners { get; private set; }

	[field: SerializeField] public Tile[] neighbors { get; private set; } = new Tile[6];
	[field: SerializeField] public Edge[] edges { get; private set; } = new Edge[6];

	/// <summary> Creates a Tile based on the given source and hex. </summary>
	public static Tile Create(TileData source, Hex hex) {
		if (Game.grid.tiles.ContainsKey(hex.pos)) throw new InvalidOperationException("There is already a Tile at the Hex.");
		return Create<Tile>(source, v => {
			v.gameObject.transform.position = Layout.HexToPoint(hex).xxy().SetY(source.container.transform.position.y);
			v.hex = hex;
			v.gameObject.transform.parent = Game.instance.transform;
			v.gameObject.name = $"Tile ({hex.x}, {hex.y})";
			Game.grid.tiles.Add(hex.pos, v);
			v.highlighter = v.gameObject.GetComponentInChildren<Highlighter>();
			if (!v.highlighter) Debug.LogError("No Highlighter in container.");
			var pt = Layout.HexToPoint(hex);
			v.center = new Vector3(pt.x, 0, pt.y);
			v.transform.position = v.center.SetY(v.transform.position.y);
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
				this.SetEdge(i, nbr.GetEdge(opposite), false);
			} else {
				Edge.Create(Game.grid.defaultEdge, this, (TileDir)i);
			}
		}
		// Create default edge modifiers
		for (int i = 0; i < neighbors.Length; i++) {
			var edge = edges[i];
			Debug.Assert(edge); // Should be set for all direction
			foreach (var edgeSource in this.data.edgeModifiers[i]) {
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

	public IEnumerable<(Tile tile, Edge edge)> NeighborsWithEdges() {
		for (int i = 0; i < neighbors.Length; i++) {
			var neighbor = neighbors[i];
			if (neighbor == null) continue;
			var edge = edges[i];
			yield return (neighbor, edge);
		}
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
}
