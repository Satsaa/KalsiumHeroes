
using System;
using System.Collections.Generic;
using System.Linq;
using Muc.Editor;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;


public abstract partial class Master<TSelf, TActor, THook> : Master
	where TSelf : Master<TSelf, TActor, THook>
	where TActor : Actor
	where THook : IHook {

	[Tooltip("Automatically created modifiers for the Master")]
	public List<RootModifier> baseModifiers;

	[Tooltip("Instantiated GameObject when the Master is shown. Actors are more defined containers.")]
	public ComponentReference<TActor> baseActor;


	public static Type actorType => typeof(TActor);
	public static Type hookType => typeof(THook);

	public ObjectDict<RootModifier> modifiers = new();
	public Hooks<THook> hooks = new();
	public override Hooks rawHooks => hooks;

	[field: SerializeField]
	public TActor actor { get; private set; }

	public GameObject gameObject => actor ? actor.gameObject : null;
	public Transform transform => actor ? actor.transform : null;


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
		if (baseActor.value) actor = Instantiate(baseActor.value);
	}

	protected override void OnHide() {
		if (actor) ObjectUtil.Destroy(gameObject);
		actor = null;
		base.OnHide();
	}

	protected void AttachModifier(RootModifier modifier) {
		modifiers.Add(modifier);
		hooks.Hook(modifier);
	}

	protected void DetachModifier(RootModifier modifier) {
		modifiers.Remove(modifier);
		hooks.Unhook(modifier);
	}

	internal override void OnActorAttach() {

	}

	internal override void OnActorDetach() {
		actor = null;
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

public abstract class Master : DataObject {

	public abstract Hooks rawHooks { get; }

	internal abstract void OnActorAttach();
	internal abstract void OnActorDetach();

}