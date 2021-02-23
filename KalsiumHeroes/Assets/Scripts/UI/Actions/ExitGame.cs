
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class ExitGame : MonoBehaviour {
	public void DoExitGame() {
		if (Application.isEditor) {
			UnityEditor.EditorApplication.ExitPlaymode();
		} else {
			Application.Quit();
		}
	}
}