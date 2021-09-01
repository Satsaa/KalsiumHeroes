
using UnityEngine;

public abstract class Actor : Hooker {

	public Animator animator;

	protected override void Awake() {
		base.Awake();
		if (!transform.parent) transform.parent = Game.game.transform;
		if (!animator) animator = GetComponent<Animator>();
	}

	protected override void OnDestroy() {
		base.OnDestroy();
		if (master) Detach();
	}

	[field: SerializeField, HideInInspector]
	public Master master { get; private set; }
	public bool attached => master;

	public virtual void Attach(Master master) {
		this.master = master;
		master.rawHooks.Hook(this);
	}
	public virtual void Detach() {
		master.OnDetach();
		master.rawHooks.Unhook(this);
		master = null;
	}

	public abstract void EndAnimations();
}

public abstract class Actor<T> : Actor where T : Master {
	new public T master => (T)base.master;
}