
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary> A hooker automatically hooks to the global hooks in Awake and OnDestroy. </summary>
public abstract class Hooker : MonoBehaviour {

	protected virtual void Awake() {
		Game.hooks.Hook(this);
	}

	protected virtual void OnDestroy() {
		if (Game.game) Game.hooks.Unhook(this);
	}

}