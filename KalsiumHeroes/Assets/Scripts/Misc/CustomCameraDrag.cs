using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Muc.Systems.Camera;

public class CustomCameraDrag : MyUnityCameraDrag {

	protected override void StartDrag() {
		if (CustomInputModule.IsPointerOverUI()) return;
		base.StartDrag();
	}

}
