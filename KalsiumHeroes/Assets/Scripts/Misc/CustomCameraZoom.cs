using System.Collections;
using System.Collections.Generic;
using Muc.Systems.Camera;
using Muc.Time;
using UnityEngine;

public class CustomCameraZoom : MyUnityCameraZoom {

	[Tooltip("Allow zooming even over UI elements after zooming.")]
	[SerializeField] private Timeout freeZoom = new Timeout(0.25f);

	new protected void Update() {
		if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() || !freeZoom.expired) {
			if (Input.mouseScrollDelta.y != 0) freeZoom.Reset();
			base.Update();
		}
	}

}
