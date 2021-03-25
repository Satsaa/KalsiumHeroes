using System.Collections;
using System.Collections.Generic;
using Muc.Systems.Camera;
using Muc.Time;
using UnityEngine;

public class CustomCameraZoom : MyUnityCameraZoom {

	[Tooltip("Allow zooming even over UI elements after zooming for this duration.")]
	[SerializeField] private Timeout freeZoom = new Timeout(0.25f);

	public override void Zoom(float amount) {
		if (!CustomInputModule.IsPointerOverUI() || !freeZoom.expired) {
			if (amount != 0) freeZoom.Reset();
			base.Zoom(amount);
		}
	}

}
