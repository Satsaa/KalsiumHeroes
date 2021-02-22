using System.Collections;
using System.Collections.Generic;
using Muc.Systems.Camera;
using Muc.Time;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomCameraZoom : MyUnityCameraZoom {

	[Tooltip("Allow zooming even over UI elements after zooming.")]
	[SerializeField] private Timeout freeZoom = new Timeout(0.25f);

	new protected void Update() {
		if (App.uibg.hovered || !freeZoom.expired) {
			if (Input.mouseScrollDelta.y != 0) freeZoom.Reset();
			base.Update();
		}
	}


}
