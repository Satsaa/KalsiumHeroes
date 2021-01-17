﻿
using System;

/// <summary>
/// An ability event handler which immediately executes doEvent.
/// </summary>
public class InstantAbilityHandler : EventHandler<Events.Ability> {

	public Ability creator;
	public Action<Ability> doEvent;
	public bool ended;

	public InstantAbilityHandler(Events.Ability msg, Ability creator, Action<Ability> handler) : base(msg) {
		this.creator = creator;
		this.doEvent = handler;
	}

	public override bool End() {
		if (!ended) doEvent(creator);
		ended = true;
		return true;
	}

	public override bool EventHasEnded() {
		return ended;
	}

	public override void Update() {
		if (ended) return;
		doEvent(creator);
		ended = true;
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
