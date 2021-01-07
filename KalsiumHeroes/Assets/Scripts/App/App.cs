using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-900)]
public class App : MonoBehaviour {

	public static App instance => _instance;
	public static Lang lang => instance._lang;

	private static App _instance;
	[SerializeField] private Lang _lang;

	private void OnValidate() => Awake();

	private void Awake() {

		if (_instance != null && _instance != this) {
			Debug.LogError($"Multiple {nameof(App)} GameObjects. Exterminating.");
			ObjectUtil.Destroy(this);
			return;
		}

		_instance = this;
	}
}
