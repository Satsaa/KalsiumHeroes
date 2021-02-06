
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Collections;
using UnityEngine.EventSystems;
using Muc.Extensions;

public class Windows : MonoBehaviour {

	public static Windows instance => _instance;
	private static Windows _instance;

	private void OnValidate() => Awake();
	private void Awake() {

		if (_instance != null && _instance != this) {
			Debug.LogError($"Multiple {nameof(Windows)} GameObjects. Exterminating.");
			ObjectUtil.Destroy(this);
			return;
		}

		_instance = this;

	}

	public void MoveToTop(Transform transform) {
		transform.SetParent(this.transform);
		transform.SetAsLastSibling();
	}
}