using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class Framerate : MonoBehaviour {

	[SerializeField] int fpsCapUnfocused = 5;
	[SerializeField] int fpsCap = 10000;


	void OnApplicationFocus(bool hasFocus) {
		if (hasFocus) {
			Application.targetFrameRate = fpsCap;
		} else {
			Application.targetFrameRate = fpsCapUnfocused;
		}
	}
}