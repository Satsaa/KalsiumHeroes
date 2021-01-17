
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class TileModifier : Modifier {

	public new TileModifierData data => (TileModifierData)base.data;
	public override Type dataType => typeof(TileModifierData);

	[HideInInspector] public Tile tile;


	protected new void Awake() {
		tile = GetMasterComponent<Tile>();
		base.Awake();
		tile.modifiers.Add(this);
		Game.dataComponents.Add(this);
		tile.onEvents.Add(this);
		OnAdd();
		foreach (var other in tile.modifiers.Get().Where(mod => mod != this)) other.OnAdd(this);
		Game.InvokeOnAfterEvent();
	}

	protected new void OnDestroy() {
		OnRemove();
		foreach (var other in tile.modifiers.Get().Where(mod => mod != this)) other.OnRemove(this);
		Game.InvokeOnAfterEvent();
		tile.onEvents.Remove(this);
		tile.modifiers.Remove(this);
		base.OnDestroy();
	}

	/// <summary> When this TileModifier is being added (instantiated). </summary>
	public virtual void OnAdd() { }
	/// <summary> When this TileModifier is being removed (destroyed). </summary>
	public virtual void OnRemove() { }

	/// <summary> When any other TileModifier is being added. </summary>
	public virtual void OnAdd(TileModifier modifier) { }
	/// <summary> When any other TileModifier is being removed. </summary>
	public virtual void OnRemove(TileModifier modifier) { }

}

