
using System;

/// <summary>
/// An ability event handler which immediately executes doEvent.
/// </summary>
public class InstantAbilityHandler : EventHandler<GameEvents.Ability> {

	public Ability creator;
	public Action<Ability> doEvent;
	public bool ended;

	public InstantAbilityHandler(GameEvents.Ability msg, Ability creator, Action<Ability> handler) : base(msg) {
		this.creator = creator;
		this.doEvent = handler;
	}

	public override bool TryEnd() {
		if (!ended) doEvent(creator);
		ended = true;
		return true;
	}

	public override bool EventHasEnded() {
		return ended;
	}

	public override void Update() {
		if (ended) return;
		ended = true;
		doEvent(creator); // 2
	}
}
