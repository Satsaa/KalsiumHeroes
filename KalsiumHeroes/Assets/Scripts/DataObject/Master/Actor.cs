
using UnityEngine;

public abstract class Actor<TMaster> : Hooker where TMaster : Master {

	[field: SerializeField, HideInInspector]
	public TMaster master { get; private set; }
	public bool attached => master;

	[field: SerializeField]
	public Animator animator { get; private set; }

	protected override void Awake() {
		base.Awake();
		if (!transform.parent) transform.parent = Game.game.transform;
		if (!animator) animator = GetComponent<Animator>();
	}

	protected override void OnDestroy() {
		base.OnDestroy();
		if (master) Detach();
	}


	public virtual void Attach(TMaster master) {
		this.master = master;
		master.rawHooks.Hook(this);
		master.OnActorAttach();
	}
	public virtual void Detach() {
		master.OnActorDetach();
		master.rawHooks.Unhook(this);
		master = null;
	}

	public abstract void EndAnimations();
}
