using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Muc.Data;
using UnityEngine.UI;
using System.Collections.ObjectModel;

[DefaultExecutionOrder(-900)]
public class App : Singleton<App> {

	public static Lang lang => instance._lang;
	public static Library library => instance._library;
	public static ReadOnlyCollection<GameMode> gameModes => instance._gameModes.AsReadOnly();

	[SerializeField] Lang _lang;
	[SerializeField] Library _library;
	[SerializeField] List<GameMode> _gameModes;

	[SerializeField] private InitDisplay initDisplay;
	[SerializeField] private SceneReference defaultScene;

	protected void OnValidate() => Awake();
	new protected void Awake() {
		base.Awake();
		if (!_library) Debug.Assert(_library = GetComponent<Library>(), this);
	}

	void Start() {
		// Load default scene if only the App scene is loaded
		if (SceneManager.sceneCount == 1) {
			initDisplay.op = SceneManager.LoadSceneAsync(defaultScene.ScenePath, LoadSceneMode.Additive);
		} else {
			Destroy(initDisplay);
		}
	}

}
