
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class ExitGame : MonoBehaviour {
	public void DoExitGame() {
#if UNITY_EDITOR
		UnityEditor.EditorApplication.ExitPlaymode();
#else
		Application.Quit();
#endif
	}
}