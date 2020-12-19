
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class TileModifier : Modifier {

	public TileModifierData tileModifierData => (TileModifierData)data;
	public override Type dataType => typeof(TileModifierData);

	public Tile tile;

	private Dictionary<object, AttributeBase> altererKeys = new Dictionary<object, AttributeBase>();


	protected new void Awake() {
		tile = GetMasterComponent<Tile>();
		base.Awake();
		tile.modifiers.Add(this);
		Game.dataComponents.Add(this);
		OnAdd();
		foreach (var other in tile.modifiers.Get().Where(mod => mod != this)) {
			other.OnAdd(this);
		}
	}

	protected new void OnDestroy() {
		OnRemove();
		foreach (var other in tile.modifiers.Get().Where(mod => mod != this)) {
			other.OnRemove(this);
		}
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

	/// <summary> When a Unit spawns on this Tile. </summary>
	public virtual void OnSpawnOn(Unit unit) { }
	/// <summary> When a Unit advances in to this Tile. (Triggered regardless of whether the Unit stops at this Tile) </summary>
	public virtual void OnMoveOn(Unit unit) { }
	/// <summary> When a Unit advances out of this Tile. (Triggered regardless of whether the Unit stops at this Tile) </summary>
	public virtual void OnMoveOff(Unit unit) { }

}

