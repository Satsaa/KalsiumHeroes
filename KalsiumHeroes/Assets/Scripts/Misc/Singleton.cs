
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour {

	public static T instance => _instance;
	private static T _instance;

	protected void OnValidate() {
		if (_instance != null && _instance != this) {
			Debug.LogError($"Multiple {typeof(T).Name} GameObjects. You are in big trouble.");
			ObjectUtil.Destroy(this);
			return;
		}
		_instance = this as T;
	}

	protected void Awake() {
		if (_instance != null && _instance != this) {
			Debug.LogError($"Multiple {typeof(T).Name} GameObjects. You are in big trouble.");
			ObjectUtil.Destroy(this);
			return;
		}
		_instance = this as T;
	}

	protected void OnDestroy() {
		if (_instance == this) {
			_instance = null;
		}
	}

}