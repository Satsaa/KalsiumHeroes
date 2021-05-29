
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Data;

public abstract class ValueHooker<T> : ValueReceiver<T> {

	protected bool hooked => hooker;

	[SerializeField, HideInInspector] Master hooker;

	protected void OnDestroy() => Unhook();

	protected void Hook(Master master) {
		if (hooker) Unhook();
		hooker = master;
		hooker.rawHooks.Hook(this);
		Game.hooks.Hook(this);
	}

	protected void Unhook() {
		if (!hooker) return;
		hooker.rawHooks.Unhook(this);
		if (Game.game && Game.hooks != null) Game.hooks.Unhook(this);
		hooker = null;
	}

}

public abstract class ValueHooker<T1, T2> : ValueReceiver<T1, T2> {

	protected bool hooked => hooker;

	[SerializeField, HideInInspector] Master hooker;

	protected void OnDestroy() => Unhook();

	protected void Hook(Master master) {
		if (hooker) Unhook();
		hooker = master;
		hooker.rawHooks.Hook(this);
		Game.hooks.Hook(this);
	}

	protected void Unhook() {
		if (!hooker) return;
		hooker.rawHooks.Unhook(this);
		if (Game.game && Game.hooks != null) Game.hooks.Unhook(this);
		hooker = null;
	}

}

public abstract class ValueHooker<T1, T2, T3> : ValueReceiver<T1, T2, T3> {

	protected bool hooked => hooker;

	[SerializeField, HideInInspector] Master hooker;

	protected void OnDestroy() => Unhook();

	protected void Hook(Master master) {
		if (hooker) Unhook();
		hooker = master;
		hooker.rawHooks.Hook(this);
		Game.hooks.Hook(this);
	}

	protected void Unhook() {
		if (!hooker) return;
		hooker.rawHooks.Unhook(this);
		if (Game.game && Game.hooks != null) Game.hooks.Unhook(this);
		hooker = null;
	}

}

public abstract class ValueHooker<T1, T2, T3, T4> : ValueReceiver<T1, T2, T3, T4> {

	protected bool hooked => hooker;

	[SerializeField, HideInInspector] Master hooker;

	protected void OnDestroy() => Unhook();

	protected void Hook(Master master) {
		if (hooker) Unhook();
		hooker = master;
		hooker.rawHooks.Hook(this);
		Game.hooks.Hook(this);
	}

	protected void Unhook() {
		if (!hooker) return;
		hooker.rawHooks.Unhook(this);
		if (Game.game && Game.hooks != null) Game.hooks.Unhook(this);
		hooker = null;
	}

}


public abstract class ValueHooker<T1, T2, T3, T4, T5> : ValueReceiver<T1, T2, T3, T4, T5> {

	protected bool hooked => hooker;

	[SerializeField, HideInInspector] Master hooker;

	protected void OnDestroy() => Unhook();

	protected void Hook(Master master) {
		if (hooker) Unhook();
		hooker = master;
		hooker.rawHooks.Hook(this);
		Game.hooks.Hook(this);
	}

	protected void Unhook() {
		if (!hooker) return;
		hooker.rawHooks.Unhook(this);
		if (Game.game && Game.hooks != null) Game.hooks.Unhook(this);
		hooker = null;
	}

}
