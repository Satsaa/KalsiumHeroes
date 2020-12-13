
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using HexGrid;

[RequireComponent(typeof(Highlighter))]
public class Tile : MonoBehaviour {

	public bool blocked;
	public float moveCost = 1;
	public float positiviness;
	public Highlighter highlighter;

	public static implicit operator Hex(Tile v) => v.hex;

	public void Awake() {
		var ob = gameObject.transform.Find("Obstacle");
		if (blocked) {
			ob.gameObject.SetActive(true);
		} else ob.gameObject.SetActive(false);
	}
	public void Init(Hex hex) {
		this.hex = hex;
		var pt = Layout.HexToPoint(hex);
		center = new Vector3(pt.x, 0, pt.y);
		transform.position = center;
		corners = Layout.Corners(hex).Select(v => new Vector3(v.x, 0, v.y)).ToArray();
	}

	public Unit unit;
	public List<GraveUnit> graveyard;

	[field: SerializeField] public Hex hex { get; private set; }

	[field: SerializeField] public Vector3 center { get; private set; }
	[field: SerializeField] public Vector3[] corners { get; private set; }

	[field: SerializeField] public Tile downRight { get; internal set; }
	[field: SerializeField] public Tile downLeft { get; internal set; }
	[field: SerializeField] public Tile left { get; internal set; }
	[field: SerializeField] public Tile upLeft { get; internal set; }
	[field: SerializeField] public Tile upRight { get; internal set; }
	[field: SerializeField] public Tile right { get; internal set; }

	public enum Dir {
		DownRight,
		DownLeft,
		Left,
		UpLeft,
		UpRight,
		Right,
	}

	public Tile GetNeighbor(Dir direction) {
		switch (direction) {
			case Dir.DownRight: return downRight;
			case Dir.DownLeft: return downLeft;
			case Dir.Left: return left;
			case Dir.UpLeft: return upLeft;
			case Dir.UpRight: return upRight;
			case Dir.Right: return right;
			default: return null;
		}
	}

	public void SetNeighbor(Dir direction, Tile value) {
		switch (direction) {
			case Dir.DownRight: downRight = value; break;
			case Dir.DownLeft: downLeft = value; break;
			case Dir.Left: left = value; break;
			case Dir.UpLeft: upLeft = value; break;
			case Dir.UpRight: upRight = value; break;
			case Dir.Right: right = value; break;
		}
	}

	public IEnumerable<Tile> Neighbors() {
		if (downRight != null) yield return downRight;
		if (downLeft != null) yield return downLeft;
		if (left != null) yield return left;
		if (upLeft != null) yield return upLeft;
		if (upRight != null) yield return upRight;
		if (right != null) yield return right;
	}

}
