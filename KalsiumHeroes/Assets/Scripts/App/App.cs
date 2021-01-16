using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Muc.Data;

[DefaultExecutionOrder(-900)]
public class App : MonoBehaviour {

	public static App instance => _instance;
	public static Lang lang => instance._lang;

	private static App _instance;
	[SerializeField] private Lang _lang;

	[SerializeField] private SceneReference defaultScene;
	[SerializeField] private InitDisplay initDisplay;
	[SerializeField] private TextProvider textTest;

	private void OnValidate() {
		if (_instance != null && _instance != this) {
			Debug.LogError($"Multiple {nameof(App)} GameObjects. Exterminating.");
			ObjectUtil.Destroy(this);
			return;
		}

		_instance = this;
	}

	private void Awake() {

		OnValidate();


	}

	private void Start() {

		if (SceneManager.sceneCount == 1) {
			initDisplay.op = SceneManager.LoadSceneAsync(defaultScene.ScenePath, LoadSceneMode.Additive);
		} else {
			Destroy(initDisplay);
		}
	}
}
