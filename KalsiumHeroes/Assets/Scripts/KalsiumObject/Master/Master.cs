
using System;
using System.Collections.Generic;
using System.Linq;
using Muc.Editor;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

public abstract class Master<TSelf, TModifier> : Master<TSelf, INoneHook, TModifier>
	where TSelf : Master<TSelf, INoneHook, TModifier>
	where TModifier : Master<TSelf, INoneHook, TModifier>.RootModifier { }

/// <summary> Variant Master with an Actor </summary>
public abstract class Master<TSelf, THook, TActor, TModifier> : Master<TSelf, THook, TModifier>
	where TSelf : Master<TSelf, THook, TActor, TModifier>
	where THook : IHook
	where TActor : Actor
	where TModifier : Master<TSelf, THook, TActor, TModifier>.RootModifier {

	[Tooltip("Instantiated GameObject when the Master is shown. Actors are more defined containers.")]
	public ComponentReference<TActor> baseActor;


	public static Type actorType => typeof(TActor);

	[field: SerializeField]
	public TActor actor { get; private set; }

	public GameObject gameObject => actor ? actor.gameObject : null;
	public Transform transform => actor ? actor.transform : null;


	protected override void OnShow() {
		base.OnShow();
		if (baseActor.value) actor = Instantiate(baseActor.value);
	}

	protected override void OnHide() {
		if (actor) ObjectUtil.Destroy(gameObject);
		actor = null;
		base.OnHide();
	}

	internal override void OnActorAttach() {

	}

	internal override void OnActorDetach() {
		actor = null;
	}

	/// <summary>
	/// Base class for all Modifiers of Masters with Actors.
	/// </summary>
	public abstract class ActorModifier : RootModifier {

		[Tooltip("If defined, when creating this Modifier, instantiate this GameObject as a child and add the Modifier to it instead.")]
		public GameObjectReference baseContainer;


		/// <summary> Optional GameObject created for this Modifier </summary>
		[field: SerializeField]
		public GameObject container { get; private set; }

		protected override void OnRemove() {
			if (container) {
				ObjectUtil.Destroy(container);
				container = null;
			}
			base.OnRemove();
		}

		protected override void OnShow() {
			base.OnShow();
			if (baseContainer.value && !master.removed) {
				Canvas canvas;
				// Create containers containing RectTransforms on the Canvas of the Master.
				container = baseContainer.GetComponent<RectTransform>() && (canvas = master.gameObject.GetComponentInChildren<Canvas>())
					? ObjectUtil.Instantiate(baseContainer, canvas.transform)
					: ObjectUtil.Instantiate(baseContainer, master.transform);
				container.transform.localRotation = baseContainer.transform.localRotation;
			}
		}

	}

}

public abstract partial class Master<TSelf, THook, TModifier> : Master
	where TSelf : Master<TSelf, THook, TModifier>
	where THook : IHook
	where TModifier : Master<TSelf, THook, TModifier>.RootModifier {

	[Tooltip("Automatically created modifiers for the Master")]
	public List<TModifier> baseModifiers;


	public static Type hookType => typeof(THook);
	public static Type modifierType => typeof(TModifier);

	public ObjectDict<TModifier> modifiers = new();
	public Hooks<THook> hooks = new();
	public override Hooks rawHooks => hooks;
	public override IObjectDict rawModifiers => modifiers;


	protected override void OnCreate() {
		hooks.Hook(this);
		foreach (var baseModifier in baseModifiers.Where(v => v != null)) {
			RootModifier.Create((TSelf)this, baseModifier);
		}
	}

	protected override void OnRemove() {
		hooks.Unhook(this);
		foreach (var modifier in modifiers.ToList()) {
			modifier.Remove();
		}
	}

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
		//
	}

	protected override void OnHide() {
		//
		base.OnHide();
	}

	internal override void OnActorAttach() { }
	internal override void OnActorDetach() { }

	protected void AttachModifier(TModifier modifier) {
		modifiers.Add(modifier);
		hooks.Hook(modifier);
	}

	protected void DetachModifier(TModifier modifier) {
		modifiers.Remove(modifier);
		hooks.Unhook(modifier);
	}

	/// <summary> Creates a Master based on the given source. </summary>
	protected static T Create<T>(T source, Action<T> initializer = null) where T : TSelf {
		if (source == null) throw new ArgumentNullException(nameof(source));
		if (!source.isSource) throw new ArgumentException($"{nameof(source)} must be a source", nameof(source));
		var master = Instantiate(source);
		master.source = source;

		Game.dataObjects.Add(master);
		Game.hooks.Hook(master);

		initializer?.Invoke(master);

		master.OnConfigureNonpersistent(true);
		master.Show();
		master.OnCreate();

		return master;
	}

}

public abstract class Master : KalsiumObject {

	public abstract Hooks rawHooks { get; }
	public abstract IObjectDict rawModifiers { get; }

	internal abstract void OnActorAttach();
	internal abstract void OnActorDetach();

}