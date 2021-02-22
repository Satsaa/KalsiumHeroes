
using System;
using System.Linq;
using Muc.Editor;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

/// <summary>
/// Base class for all Modifiers of Masters.
/// </summary>
public abstract class Modifier : DataObject {

	public new ModifierData source => (ModifierData)_source;
	public new ModifierData data => (ModifierData)_data;
	public override Type dataType => typeof(ModifierData);

	[Tooltip("Master component for this Modifier."), SerializeField]
	Master _master;

	public Master master => _master;

	/// <summary> Optional GameObject created for this Modifier </summary>
	[field: SerializeField]
	public GameObject container { get; private set; }

	/// <summary> A virtual Modifier is wrapped by a Virtualizer which acts as a layer. </summary>
	[HideInInspector, SerializeField] public bool virtualized;

	/// <summary> Removes this Modifier </summary>
	public void Remove() {
		if (removed) return;
		removed = true;

		master.OnRemove(this);
		Game.dataObjects.Remove(this);
		Game.onEvents.Remove(this);

		OnConfigureNonpersistent(false);
		OnRemove();
		if (container) ObjectUtil.Destroy(container);
	}

	/// <summary> Creates a Modifier based on the given source and attaches it to the master. </summary>
	public static Modifier Create(Master master, ModifierData source, Action<Modifier> initializer = null) {
		return Create<Modifier>(master, source, initializer);
	}
	/// <summary> Creates a Modifier based on the given source and attaches it to the master. </summary>
	public static T Create<T>(Master master, ModifierData source, Action<T> initializer = null) where T : Modifier {
		var modifier = (T)CreateInstance(source.createType);
		modifier._master = master;
		modifier._source = source;
		modifier._data = Instantiate(source);
		if (source.container) {
			Canvas canvas;
			// Create containers containing RectTransforms on the Canvas of the Master.
			if (source.container.GetComponent<RectTransform>() && (canvas = master.gameObject.GetComponentInChildren<Canvas>())) {
				modifier.container = ObjectUtil.Instantiate(source.container, canvas.transform);
			} else {
				modifier.container = ObjectUtil.Instantiate(source.container, master.gameObject.transform);
			}
			modifier.container.transform.rotation = source.container.transform.localRotation;
		}

		master.OnCreate(modifier);
		Game.dataObjects.Add(modifier);
		Game.onEvents.Add(modifier);

		initializer?.Invoke(modifier);

		modifier.OnConfigureNonpersistent(true);
		modifier.OnCreate();

		return modifier;
	}

	protected virtual void DealDamage(Unit unit, float damage, DamageType type) {
		unit.DealCalculatedDamage(this, damage, type);
	}

	public void ExecuteMoveOver(Unit unit, Tile from, Edge edge, Tile to) {
		edge = from.EdgeBetween(to);
		using (var scope = new OnEvents.Scope()) {
			edge.onEvents.ForEach<IOnMoveOver_Edge>(scope, v => v.OnMoveOver(this, unit, from, to));
			unit.onEvents.ForEach<IOnMoveOver_Unit>(scope, v => v.OnMoveOver(this, from, edge, to));
			Game.onEvents.ForEach<IOnMoveOver_Global>(scope, v => v.OnMoveOver(this, unit, from, edge, to));
		}
	}

	public void ExecuteMoveOver(Unit unit, Tile from, Tile to) {
		var edge = from.EdgeBetween(to);
		using (var scope = new OnEvents.Scope()) {
			edge.onEvents.ForEach<IOnMoveOver_Edge>(scope, v => v.OnMoveOver(this, unit, from, to));
			unit.onEvents.ForEach<IOnMoveOver_Unit>(scope, v => v.OnMoveOver(this, from, edge, to));
			Game.onEvents.ForEach<IOnMoveOver_Global>(scope, v => v.OnMoveOver(this, unit, from, edge, to));
		}
	}

	public void ExecuteMoveOn(Unit unit, Tile tile) {
		using (var scope = new OnEvents.Scope()) {
			tile.onEvents.ForEach<IOnMoveOn_Tile>(scope, v => v.OnMoveOn(this, unit));
			unit.onEvents.ForEach<IOnMoveOn_Unit>(scope, v => v.OnMoveOn(this, tile));
			Game.onEvents.ForEach<IOnMoveOn_Global>(scope, v => v.OnMoveOn(this, unit, tile));
		}
	}
	public void ExecuteMoveOff(Unit unit, Tile tile) {
		using (var scope = new OnEvents.Scope()) {
			tile.onEvents.ForEach<IOnMoveOff_Tile>(scope, v => v.OnMoveOff(this, unit));
			unit.onEvents.ForEach<IOnMoveOff_Unit>(scope, v => v.OnMoveOff(this, tile));
			Game.onEvents.ForEach<IOnMoveOff_Global>(scope, v => v.OnMoveOff(this, unit, tile));
		}
	}
}