using System.Collections;
using System.Collections.Generic;
using Muc.Systems.Camera;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomCameraDrag : MyUnityCameraDrag {

	[SerializeField, HideInInspector] private bool dragging;

	new protected void Update() {
		if (Input.GetKeyDown(key) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) StartDrag();
		if (Input.GetKey(key)) UpdateDrag();
		else dragging = false;
	}

	public override void StartDrag() {
		dragging = true;
		base.StartDrag();

	}

	public override void UpdateDrag() {
		if (dragging) {
			base.UpdateDrag();
		}
	}

}
