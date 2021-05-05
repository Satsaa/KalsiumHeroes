
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public abstract class HookBehaviour : MonoBehaviour {

	protected void Awake() {
		Game.onEvents.Add(this);
	}

	protected void OnDestroy() {
		Game.onEvents.Remove(this);
	}

}