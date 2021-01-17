
using System;

/// <summary>
/// An ability event handler which immediately executes doEvent.
/// </summary>
public class InstantAbilityHandler : EventHandler<Events.Ability> {

	public InstantAbilityHandler(Events.Ability msg, Ability creator, Action<Ability> doEvent) : base(msg) {
		doEvent(creator);
	}

	public override bool End() {
		return true;
	}

	public override bool EventHasEnded() {
		return true;
	}

	public override void Update() {

	}
}

/// <summary>
/// An ability event handler which immediately executes doEvent.
/// </summary>
public class InstantAbilityHandler<T> : EventHandler<Events.Ability> where T : Ability {

	public InstantAbilityHandler(Events.Ability msg, T creator, Action<T> doEvent) : base(msg) {
		doEvent(creator);
	}

	public override bool End() {
		return true;
	}

	public override bool EventHasEnded() {
		return true;
	}

	public override void Update() {

	}
}
