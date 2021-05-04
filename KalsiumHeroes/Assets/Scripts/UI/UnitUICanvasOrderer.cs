using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Muc.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class UnitUICanvasOrderer : MonoBehaviour {

	static List<UnitUICanvasOrderer> orderers = new List<UnitUICanvasOrderer>();

	[SerializeField] Canvas canvas;
	private float distance;

	void OnValidate() => Init();
	void Reset() => Init();
	void Start() => Init();

	void Init() {
		if (!canvas) canvas = GetComponentInChildren<Canvas>();
	}

	void Update() {
		orderers.Add(this);
	}

	void LateUpdate() {
		distance = Vector3.Distance(transform.position, Camera.main.transform.position);
		if (this == orderers.Last()) {
			orderers.Sort((a, b) => a.distance.CompareTo(b.distance));
			for (int i = 0; i < orderers.Count; i++) {
				orderers[i].canvas.sortingOrder = -i - 100;
			}
			orderers.Clear();
		}
	}
}
