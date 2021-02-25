
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Data;
using UnityEngine.SceneManagement;

public class CreateLobby : MonoBehaviour {

	public SceneReference scene;

	public void DoCreateLobby() {
		SceneManager.UnloadSceneAsync(gameObject.scene);
		SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
	}
}