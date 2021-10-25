
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.Events;

using Muc.Extensions;
using UnityEngine.EventSystems;

public class SpawnControl : DraggableObject {

	protected static List<SpawnControl> positioners = new();

	new public Camera camera;
	public Unit source;
	public Team team;
	public Vector3 dragOffset = new(0, 0.2f, 0);

	public Tile tile;
	public Tile startTile;
	public Tile prevTile;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	static void SubsystemRegistration() {
		positioners = new List<SpawnControl>();
	}

	protected void Awake() {
		if (!camera) Debug.Assert(camera = Camera.main);
	}

	protected void OnEnable() {
		positioners.Add(this);
		if (!tile) SetTile(Game.grid.tiles.Values.First(v => positioners.All(vv => vv.tile != v)));
	}

	protected void OnDisable() {
		tile = null;
		positioners.Remove(this);
	}

	public override void OnDragStart(PointerEventData eventData) {
		startTile = tile;
	}

	public override void OnDrag() {

		var plane = new Plane(Vector3.up, 0);
		var ray = camera.ScreenPointToRay(App.input.pointer);
		if (plane.Raycast(ray, out var enter)) {
			transform.position = ray.origin + ray.direction * enter;
		}

		var newTile = Game.grid.GetHoveredTile();
		if (newTile) {
			if (newTile != startTile)
				transform.position = newTile.center + dragOffset;
			if (prevTile != newTile) {
				if (prevTile) prevTile.highlighter.Unhighlight(Highlighter.hoverPriority);
				prevTile = newTile;
			}
			newTile.highlighter.Highlight(Highlighter.hoverColor, Highlighter.hoverPriority);
		}
	}

	public override void OnDragEnd(PointerEventData eventData) {
		var newTile = Game.grid.GetHoveredTile(camera);
		if (prevTile) prevTile.highlighter.Unhighlight(Highlighter.hoverPriority);
		if (newTile) SetTile(newTile);
	}

	public void SetTile(Tile newTile) {
		if (tile == newTile) return;
		prevTile = tile;

		SpawnControl occupier = default;
		foreach (var poser in positioners) {
			if (poser != this && poser.tile == newTile) {
				occupier = poser;
				break;
			}
		}
		if (occupier) {
			occupier.tile = prevTile;
			occupier.transform.position = prevTile.center;
		}

		tile = newTile;
		transform.position = newTile.center;
	}

}