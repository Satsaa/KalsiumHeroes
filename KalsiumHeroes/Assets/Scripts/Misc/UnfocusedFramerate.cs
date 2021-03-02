using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class UnfocusedFramerate : MonoBehaviour {

	public int fpsCap = 5;

	public int targetFrameRate;
	public bool started;


	void Start() {
		started = true;
		targetFrameRate = Application.targetFrameRate;
	}

	void OnApplicationFocus(bool hasFocus) {
		if (!started) return;
		if (hasFocus) {
			Application.targetFrameRate = targetFrameRate;
		} else {
			targetFrameRate = Application.targetFrameRate;
			Application.targetFrameRate = fpsCap;
		}
	}
}