
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

	/// <summary> Removes this Modifier from the Master and the game. </summary>
	public void Remove() {
		if (removed) return;
		removed = true;
		Hide();

		master.DetachModifier(this);
		Game.dataObjects.Remove(this);
		Game.hooks.Unhook(this);

		if (!master.removed) using (var scope = new Hooks.Scope()) Game.hooks.ForEach<IOnModifierRemove>(scope, v => v.OnModifierRemove(this));

		OnConfigureNonpersistent(false);
		if (!master.removed) OnRemove();
		if (container) {
			ObjectUtil.Destroy(container);
			container = null;
		}
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

		master.AttachModifier(modifier);
		Game.dataObjects.Add(modifier);
		Game.hooks.Hook(modifier);

		initializer?.Invoke(modifier);

		modifier.OnConfigureNonpersistent(true);
		modifier.OnCreate();
		modifier.Show();

		using (var scope = new Hooks.Scope()) Game.hooks.ForEach<IOnModifierCreate>(scope, v => v.OnModifierCreate(modifier));

		return modifier;
	}

	protected override void OnShow() {
		base.OnShow();
		if (source.container) {
			Canvas canvas;
			// Create containers containing RectTransforms on the Canvas of the Master.
			if (source.container.GetComponent<RectTransform>() && (canvas = master.gameObject.GetComponentInChildren<Canvas>())) {
				container = ObjectUtil.Instantiate(source.container, canvas.transform);
			} else {
				container = ObjectUtil.Instantiate(source.container, master.transform);
			}
			container.transform.localRotation = source.container.transform.localRotation;
		}
	}

	protected override void OnHide() {
		if (container) {
			ObjectUtil.Destroy(container);
			container = null;
		}
		base.OnHide();
	}

	protected virtual void DealDamage(Unit unit, float damage, DamageType type) {
		unit.DealCalculatedDamage(this, damage, type);
	}

	public void ExecuteMoveOver(Unit unit, Tile from, Edge edge, Tile to) {
		edge = from.EdgeBetween(to);
		using (var scope = new Hooks.Scope()) {
			edge.hooks.ForEach<IOnMoveOver_Edge>(scope, v => v.OnMoveOver(this, unit, from, to));
			unit.hooks.ForEach<IOnMoveOver_Unit>(scope, v => v.OnMoveOver(this, from, edge, to));
			Game.hooks.ForEach<IOnMoveOver_Global>(scope, v => v.OnMoveOver(this, unit, from, edge, to));
		}
	}

	public void ExecuteMoveOver(Unit unit, Tile from, Tile to) {
		var edge = from.EdgeBetween(to);
		using (var scope = new Hooks.Scope()) {
			edge.hooks.ForEach<IOnMoveOver_Edge>(scope, v => v.OnMoveOver(this, unit, from, to));
			unit.hooks.ForEach<IOnMoveOver_Unit>(scope, v => v.OnMoveOver(this, from, edge, to));
			Game.hooks.ForEach<IOnMoveOver_Global>(scope, v => v.OnMoveOver(this, unit, from, edge, to));
		}
	}

	public void ExecuteMoveOn(Unit unit, Tile tile) {
		using (var scope = new Hooks.Scope()) {
			tile.hooks.ForEach<IOnMoveOn_Tile>(scope, v => v.OnMoveOn(this, unit));
			unit.hooks.ForEach<IOnMoveOn_Unit>(scope, v => v.OnMoveOn(this, tile));
			Game.hooks.ForEach<IOnMoveOn_Global>(scope, v => v.OnMoveOn(this, unit, tile));
		}
	}
	public void ExecuteMoveOff(Unit unit, Tile tile) {
		using (var scope = new Hooks.Scope()) {
			tile.hooks.ForEach<IOnMoveOff_Tile>(scope, v => v.OnMoveOff(this, unit));
			unit.hooks.ForEach<IOnMoveOff_Unit>(scope, v => v.OnMoveOff(this, tile));
			Game.hooks.ForEach<IOnMoveOff_Global>(scope, v => v.OnMoveOff(this, unit, tile));
		}
	}
}
