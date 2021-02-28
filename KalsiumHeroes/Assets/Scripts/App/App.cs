using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Muc.Data;
using UnityEngine.UI;
using System.Collections.ObjectModel;

[DefaultExecutionOrder(-900)]
[RequireComponent(typeof(Library))]
[RequireComponent(typeof(Client))]
public class App : Singleton<App> {

	public static Client client => instance._client;
	public static Library library => instance._library;
	public static Lang lang => instance._lang;
	public static ReadOnlyCollection<GameMode> gameModes => instance._gameModes.AsReadOnly();

	[SerializeField, HideInInspector] Client _client;
	[SerializeField, HideInInspector] Library _library;
	[SerializeField] Lang _lang;
	[SerializeField] List<GameMode> _gameModes;

	[SerializeField] private InitDisplay initDisplay;
	[SerializeField] private SceneReference defaultScene;

	protected void Reset() {
		_library = GetComponent<Library>();
		_client = GetComponent<Client>();
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
