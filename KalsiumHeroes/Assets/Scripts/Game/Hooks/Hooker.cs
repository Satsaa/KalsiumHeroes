
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary> A hooker automatically hooks to the global hooks in Awake and OnDestroy. </summary>
public abstract class Hooker : MonoBehaviour {

	protected void Awake() {
		Game.hooks.Hook(this);
	}

	protected void OnDestroy() {
		if (Game.game) Game.hooks.Unhook(this);
	}

}