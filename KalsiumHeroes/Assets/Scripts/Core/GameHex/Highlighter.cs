
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using HexGrid;
using UnityEngine.Serialization;

public class Highlighter : MonoBehaviour {

	[SerializeField] new Renderer renderer;
	[SerializeField, HideInInspector] Color color;

	void Start() {
		renderer = GetComponentInChildren<Renderer>();
		color = renderer.material.color;
	}

	public void Highlight(Color color) {
		renderer.material.color = color;
	}

	public void ResetHighlight() {
		renderer.material.color = this.color;
	}
}
