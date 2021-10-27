
using System;
using System.Collections.Generic;
using System.Linq;
using Muc.Editor;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;


public abstract partial class Master<TSelf, TActor, THook> : Master
	where TSelf : Master<TSelf, TActor, THook>
	where TActor : Actor<TSelf>
	where THook : IHook {

	public static Type actorType => typeof(TActor);
	public static Type hookType => typeof(THook);

	public ObjectDict<BaseModifier> modifiers = new();
	public Hooks<THook> hooks = new();
	public override Hooks rawHooks => hooks;

	[Tooltip("Automatically created modifiers for the Master")]
	public List<BaseModifier> baseModifiers;

	protected override void OnCreate() {
		if (true) {
			if (true) {

			}
		}
		hooks.Hook(this);
		foreach (var baseModifier in baseModifiers.Where(v => v != null)) {
			BaseModifier.Create((TSelf)this, baseModifier);
		}
	}

	protected override void OnRemove() {
		hooks.Unhook(this);
		foreach (var modifier in modifiers.ToList()) {
			modifier.Remove();
		}
	}

	internal void AttachModifier(BaseModifier modifier) {
		modifiers.Add(modifier);
		hooks.Hook(modifier);
	}

	internal void DetachModifier(BaseModifier modifier) {
		modifiers.Remove(modifier);
		hooks.Unhook(modifier);
	}



	[Tooltip("Instantiated GameObject when the Master is shown. Actors are more defined containers.")]
	public ComponentReference<TActor> baseActor;


	[field: SerializeField]
	public TActor actor { get; private set; }

	public GameObject gameObject => actor ? actor.gameObject : null;
	public Transform transform => actor ? actor.transform : null;

	public void Remove() {
		if (removed) return;
		removed = true;

		Game.dataObjects.Remove(this);
		Game.hooks.Unhook(this);

		OnConfigureNonpersistent(false);
		OnRemove();
	}

	protected override void OnShow() {
		base.OnShow();
		if (baseActor.value) actor = Instantiate(baseActor.value);
	}

	protected override void OnHide() {
		if (actor) Destroy(gameObject);
		actor = null;
		base.OnHide();
	}

	protected internal override void OnActorAttach() {

	}

	protected internal override void OnActorDetach() {
		actor = null;
	}

	/// <summary> Creates a Master based on the given source. </summary>
	protected static T Create<T>(T source, Action<T> initializer = null) where T : TSelf {
		if (source == null) throw new ArgumentNullException(nameof(source));
		if (source.isSource) throw new ArgumentException($"{nameof(source)} must be a source", nameof(source));
		if (source.GetType() != typeof(T)) throw new ArgumentException($"{nameof(source)} must be of equivalent type as {nameof(T)}", nameof(source));
		var master = Instantiate(source);
		master.source = source;

		Game.dataObjects.Add(master);
		Game.hooks.Hook(master);

		initializer?.Invoke(master);

		master.OnConfigureNonpersistent(true);
		master.OnCreate();
		master.Show();

		return master;
	}



	/// <summary>
	/// Base class for all Modifiers of Masters.
	/// </summary>
	public abstract class BaseModifier : Modifier {

		[Tooltip("If defined, when creating this Modifier, instantiate this GameObject as a child and add the Modifier to it instead.")]
		public GameObjectReference baseContainer;


		[field: Tooltip("Master component for this Modifier."), SerializeField]
		public TSelf master { get; private set; }

		/// <summary> Optional GameObject created for this Modifier </summary>
		[field: SerializeField]
		public GameObject container { get; private set; }

		/// <summary> A virtual Modifier is wrapped by a Virtualizer which acts as a layer (WIP). </summary>
		[HideInInspector] public bool virtualized;

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
		public static T Create<T>(TSelf master, T source, Action<T> initializer = null) where T : BaseModifier {
			if (source == null) throw new ArgumentNullException(nameof(source));
			if (source.isSource) throw new ArgumentException($"{nameof(source)} must be a source", nameof(source));
			if (source.GetType() != typeof(T)) throw new ArgumentException($"{nameof(source)} must be of equivalent type as {nameof(T)}", nameof(source));

			var modifier = Instantiate(source);
			modifier.master = master;
			modifier.source = source;

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
			if (baseContainer) {
				Canvas canvas;
				// Create containers containing RectTransforms on the Canvas of the Master.
				if (baseContainer.GetComponent<RectTransform>() && (canvas = master.gameObject.GetComponentInChildren<Canvas>())) {
					container = ObjectUtil.Instantiate(baseContainer, canvas.transform);
				} else {
					container = ObjectUtil.Instantiate(baseContainer, master.transform);
				}
				container.transform.localRotation = baseContainer.transform.localRotation;
			}
		}

		protected override void OnHide() {
			if (container) {
				ObjectUtil.Destroy(container);
				container = null;
			}
			base.OnHide();
		}

		public void ExecuteMoveOver(Unit unit, Tile from, Edge edge, Tile to) {
			edge = from.EdgeBetween(to);
			using (var scope = new Hooks.Scope()) {
				edge.hooks.ForEach<IOnMoveOver_Edge>(scope, v => v.OnMoveOver(this, unit, from, to));
				unit.hooks.ForEach<IOnMoveOver_Unit>(scope, v => v.OnMoveOver(this, from, edge, to));
				Game.hooks.ForEach<IOnMoveOver_Game>(scope, v => v.OnMoveOver(this, unit, from, edge, to));
			}
		}

		public void ExecuteMoveOver(Unit unit, Tile from, Tile to) {
			var edge = from.EdgeBetween(to);
			using (var scope = new Hooks.Scope()) {
				edge.hooks.ForEach<IOnMoveOver_Edge>(scope, v => v.OnMoveOver(this, unit, from, to));
				unit.hooks.ForEach<IOnMoveOver_Unit>(scope, v => v.OnMoveOver(this, from, edge, to));
				Game.hooks.ForEach<IOnMoveOver_Game>(scope, v => v.OnMoveOver(this, unit, from, edge, to));
			}
		}

		public void ExecuteMoveOn(Unit unit, Tile tile) {
			using (var scope = new Hooks.Scope()) {
				tile.hooks.ForEach<IOnMoveOn_Tile>(scope, v => v.OnMoveOn(this, unit));
				unit.hooks.ForEach<IOnMoveOn_Unit>(scope, v => v.OnMoveOn(this, tile));
				Game.hooks.ForEach<IOnMoveOn_Game>(scope, v => v.OnMoveOn(this, unit, tile));
			}
		}
		public void ExecuteMoveOff(Unit unit, Tile tile) {
			using (var scope = new Hooks.Scope()) {
				tile.hooks.ForEach<IOnMoveOff_Tile>(scope, v => v.OnMoveOff(this, unit));
				unit.hooks.ForEach<IOnMoveOff_Unit>(scope, v => v.OnMoveOff(this, tile));
				Game.hooks.ForEach<IOnMoveOff_Game>(scope, v => v.OnMoveOff(this, unit, tile));
			}
		}
	}

}

public abstract class Modifier : DataObject {

}

public abstract class Master : DataObject {

	public abstract Hooks rawHooks { get; }

	protected internal abstract void OnActorAttach();
	protected internal abstract void OnActorDetach();
}