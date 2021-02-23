using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Muc.Data;

[DefaultExecutionOrder(-900)]
public class App : Singleton<App> {

	public static Lang lang => instance._lang;
	public static UIBG uibg => instance._uibg;

	[SerializeField] private Lang _lang;
	[SerializeField] private UIBG _uibg;
	[SerializeField] private InitDisplay initDisplay;

	[SerializeField] private SceneReference defaultScene;

	private void Start() {
		// Load default scene if only the App scene is loaded
		if (SceneManager.sceneCount == 1) {
			initDisplay.op = SceneManager.LoadSceneAsync(defaultScene.ScenePath, LoadSceneMode.Additive);
		} else {
			Destroy(initDisplay);
		}
	}
}
