
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class TileModifier : Modifier {

	public new TileModifierData source => (TileModifierData)_source;
	public new TileModifierData data => (TileModifierData)_data;
	public override Type dataType => typeof(TileModifierData);
	public Tile tile => (Tile)master;


	protected new void OnCreate() {
		base.OnCreate();
		foreach (var other in tile.modifiers.Get().Where(mod => mod != this)) other.OnCreate(this);
	}

	protected new void OnRemove() {
		foreach (var other in tile.modifiers.Get().Where(mod => mod != this)) other.OnRemove(this);
		base.OnRemove();
	}

	/// <summary> When any other TileModifier is being added. </summary>
	public virtual void OnCreate(TileModifier modifier) { }
	/// <summary> When any other TileModifier is being removed. </summary>
	public virtual void OnRemove(TileModifier modifier) { }

}

