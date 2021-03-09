
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.Events;
using Muc.Extensions;

public class SpawnControl : DraggableObject {

	protected static List<SpawnControl> positioners = new List<SpawnControl>();

	public UnitData source;
	public Team team;

	[HideInInspector] public Tile tile;
	[HideInInspector] public Tile startTile;
	[HideInInspector] public Tile prevTile;


	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	static void SubsystemRegistration() {
		positioners = new List<SpawnControl>();
	}

	new protected void Reset() {
		onDrag ??= new UnityEvent();
		onDragEnd ??= new UnityEvent();
#if UNITY_EDITOR
		UnityEditor.Events.UnityEventTools.AddPersistentListener(onDrag, OnDrag);
		UnityEditor.Events.UnityEventTools.AddPersistentListener(onDragEnd, OnDragEnd);
#else
		onDrag.AddListener(OnDrag);
		onDragEnd.AddListener(OnDragEnd);
#endif
	}

	new protected void Awake() {
		if (!resetCalled) Reset();
		camera = Camera.main;
	}

	protected void OnEnable() {
		positioners.Add(this);
		if (!tile) SetTile(Game.grid.tiles.Values.First(v => positioners.All(vv => vv.tile != v)));
	}
	protected void OnDisable() {
		tile = null;
		positioners.Remove(this);
	}

	public override void OnDrag() {
		var plane = new Plane(Vector3.up, 0);
		var ray = camera.ScreenPointToRay(Input.mousePosition);
		if (plane.Raycast(ray, out var enter)) {
			transform.position = ray.origin + ray.direction * enter + dragOffset;
		}
		var newTile = Game.grid.GetHoveredTile(camera);
		if (prevTile != newTile) {
			if (prevTile) prevTile.highlighter.Unhighlight(Highlighter.hoverPriority);
			prevTile = newTile;
		}
		if (newTile) newTile.highlighter.Highlight(Highlighter.hoverColor, Highlighter.hoverPriority);
	}

	public void OnDragEnd() {
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