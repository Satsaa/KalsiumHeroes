
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[DefaultExecutionOrder(999999)]
public class SharedInput : MonoBehaviour {

	/// <summary> The position of the pointer (mouse) </summary>
	[field: SerializeField, HideInInspector] public Vector2 pointer { get; private set; }

	/// <summary> The state of the primary button (left mouse button) </summary>
	[field: SerializeField, HideInInspector] public bool primary { get; private set; }
	[field: SerializeField, HideInInspector] public bool primaryUp { get; private set; }
	[field: SerializeField, HideInInspector] public bool primaryDown { get; private set; }

	/// <summary> The state of the secondary button (right mouse button) </summary>
	[field: SerializeField, HideInInspector] public bool secondary { get; private set; }
	[field: SerializeField, HideInInspector] public bool secondaryUp { get; private set; }
	[field: SerializeField, HideInInspector] public bool secondaryDown { get; private set; }

	public void LateUpdate() {
		primaryDown = secondaryDown = false;
		primaryUp = secondaryUp = false;
	}


	public void SetPointer(Vector2 position) {
		this.pointer = position;
	}

	public void SetPrimary(bool value) {
		primary = value;
		if (value) {
			primaryDown = true;
		} else {
			primaryUp = true;
		}
	}

	public void SetSecondary(bool value) {
		secondary = value;
		if (value) {
			secondaryDown = true;
		} else {
			secondaryUp = true;
		}
	}
}