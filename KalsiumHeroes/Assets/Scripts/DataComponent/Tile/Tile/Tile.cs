
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using HexGrid;
using Muc.Extensions;
using Muc.Numerics;

[ExecuteAlways]
public class Tile : MasterComponent<TileModifier> {

	public static implicit operator Hex(Tile v) => v.hex;

	public TileData tileData => (TileData)data;
	public override Type dataType => typeof(TileData);

	public Highlighter highlighter;

	public Unit unit;
	public List<GraveUnit> graveyard;

	[field: SerializeField] public Hex hex { get; private set; }

	[field: SerializeField] public Vector3 center { get; private set; }
	[field: SerializeField] public Vector3[] corners { get; private set; }

	[field: SerializeField] public Tile[] neighbors { get; private set; } = new Tile[6];
	[field: SerializeField] public Edge[] edges { get; private set; } = new Edge[6];

	public bool awoken = false;

	protected new void Awake() {
		awoken = true;
		if (!source) throw new InvalidOperationException("Source must be defined when creating a Tile!");
		base.Awake();
		hex = Layout.PointToHex(transform.position.xz()).Round();
		highlighter = GetComponentInChildren<Highlighter>();
		if (!highlighter) Debug.LogError("No Highlighter in MasterComponent instantiatee.");
		var pt = Layout.HexToPoint(hex);
		center = new Vector3(pt.x, 0, pt.y);
		transform.position = center.SetY(transform.position.y);
		corners = Layout.Corners(hex).Select(v => new Vector3(v.x, 0, v.y)).ToArray();
	}

	protected new void OnDestroy() {
		for (int i = 0; i < neighbors.Length; i++) {
			var nbr = neighbors[i];
			var edge = edges[i];
			if (edge) {
				if (nbr == null) {
					ObjectUtil.Destroy(edge.gameObject);
				} else {
					edge.RemoveModifiersByContext(this);
				}
			}
		}
		base.OnDestroy();
	}

	public IEnumerable<Tile> Neighbors() => neighbors.Where(v => v != null);

	public Tile GetNeighbor(TileDir direction) => neighbors[(int)direction];
	public Tile GetNeighbor(int direction) => neighbors[direction];

	public void SetNeighbor(TileDir direction, Tile tile, bool propagate = true) => SetNeighbor((int)direction, tile, propagate);
	public void SetNeighbor(int direction, Tile tile, bool propagate = true) {
		neighbors[direction] = tile;
		if (!propagate) return;
		var neighbor = GetNeighbor(direction);
		if (neighbor) neighbor.SetNeighbor(new CircularInt(direction - 3, 6), this, false);
	}

	public IEnumerable<Edge> Edges() => edges.Where(v => v != null);

	public Edge GetEdge(TileDir direction) => edges[(int)direction];
	public Edge GetEdge(int direction) => edges[direction];

	public void SetEdge(TileDir direction, Edge edge, bool propagate = true) => SetEdge((int)direction, edge, propagate);
	public void SetEdge(int direction, Edge edge, bool propagate = true) {
		edges[direction] = edge;
		if (!propagate) return;
		edge.tile1 = this;
		var neighbor = GetNeighbor(direction);
		edge.tile2 = neighbor;
		if (neighbor) neighbor.SetEdge(new CircularInt(direction - 3, 6), edge, false);
	}
}
