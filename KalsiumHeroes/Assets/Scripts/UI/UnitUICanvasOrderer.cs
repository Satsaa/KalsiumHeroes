using System.Collections;
using System.Collections.Generic;
using Muc.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class UnitUICanvasOrderer : MonoBehaviour {

	public Canvas canvas;

	void OnValidate() => Init();
	void Reset() => Init();
	void Start() => Init();

	void Init() {
		if (!canvas) canvas = GetComponentInChildren<Canvas>();
	}

	void LateUpdate() {
		var distance = Vector3.Distance(transform.position, Camera.main.transform.position);
		canvas.sortingOrder = (int)(distance * -25);
	}
}
