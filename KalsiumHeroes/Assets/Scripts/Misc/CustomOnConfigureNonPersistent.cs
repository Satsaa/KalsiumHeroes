
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public interface ICustomOnConfigureNonPersistent {
	void OnConfigureNonpersistent(bool add);
}

public abstract class CustomOnConfigureNonPersistent : MonoBehaviour, ICustomOnConfigureNonPersistent {

	protected virtual void Awake() {
		OnConfigureNonpersistent(true);
	}

	protected virtual void OnDestroy() {
		OnConfigureNonpersistent(false);
	}

	void ICustomOnConfigureNonPersistent.OnConfigureNonpersistent(bool add) => OnConfigureNonpersistent(add);
	protected abstract void OnConfigureNonpersistent(bool add);

#if UNITY_EDITOR
	[UnityEditor.Callbacks.DidReloadScripts]
	private static void OnReloadScripts() {
		if (Application.isPlaying && Game.game) {
			foreach (var dobj in FindObjectsOfType<Object>(true).OfType<ICustomOnConfigureNonPersistent>()) {
				dobj.OnConfigureNonpersistent(true);
			}
		}
	}

#endif
}