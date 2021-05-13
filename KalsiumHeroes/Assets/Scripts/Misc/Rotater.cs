using System.Collections;
using System.Collections.Generic;
using Muc.Extensions;
using UnityEngine;

public class Rotater : MonoBehaviour {

	public float mult = 1;

	public void OnPoint(Vector3 pointer) {
		OnPoint(pointer.xy());
	}

	public void OnPoint(Vector2 pointer) {
		var res = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
		var normalized = (pointer / res).Add(-0.5f);
		transform.localRotation = Quaternion.Euler(normalized.x * mult, 0, normalized.y * mult);
	}

}
