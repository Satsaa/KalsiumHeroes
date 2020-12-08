
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using HexGrid;
using UnityEngine.Serialization;

[RequireComponent(typeof(Highlighter))]
public class GameHex : MonoBehaviour {

	public bool blocked;
	public float moveCost = 1;
	public float positiviness;
	public Highlighter highlighter;

	public void Awake() {
		var ob = gameObject.transform.Find("Obstacle");
		if (blocked) {
			ob.gameObject.SetActive(true);
		} else ob.gameObject.SetActive(false);
	}
	public void Init(Hex hex) {
		this.hex = hex;
		var pix = Layout.HexToPixel(hex);
		center = new Vector3(pix.x, 0, pix.y);
		transform.position = center;
		corners = Layout.PolygonCorners(hex).Select(v => new Vector3(v.x, 0, v.y)).ToArray();
	}

	public Unit unit;
	public List<GraveUnit> graveYard;

	[field: SerializeField] public Hex hex { get; private set; }

	[field: SerializeField] public Vector3 center { get; private set; }
	[field: SerializeField] public Vector3[] corners { get; private set; }

	[field: SerializeField] public GameHex downRight { get; internal set; }
	[field: SerializeField] public GameHex downLeft { get; internal set; }
	[field: SerializeField] public GameHex left { get; internal set; }
	[field: SerializeField] public GameHex upLeft { get; internal set; }
	[field: SerializeField] public GameHex upRight { get; internal set; }
	[field: SerializeField] public GameHex right { get; internal set; }

	public enum Dir {
		DownRight,
		DownLeft,
		Left,
		UpLeft,
		UpRight,
		Right,
	}

	public GameHex GetNeighbor(Dir direction) {
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

	public void SetNeighbor(Dir direction, GameHex value) {
		switch (direction) {
			case Dir.DownRight: downRight = value; break;
			case Dir.DownLeft: downLeft = value; break;
			case Dir.Left: left = value; break;
			case Dir.UpLeft: upLeft = value; break;
			case Dir.UpRight: upRight = value; break;
			case Dir.Right: right = value; break;
		}
	}

	public IEnumerable<GameHex> Neighbors() {
		if (downRight != null) yield return downRight;
		if (downLeft != null) yield return downLeft;
		if (left != null) yield return left;
		if (upLeft != null) yield return upLeft;
		if (upRight != null) yield return upRight;
		if (right != null) yield return right;
	}

}
