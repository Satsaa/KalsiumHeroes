using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Muc.Extensions;
using UnityEngine.UI;
using UnityEngine;

public class StatsUIModifier : UnitModifier {

	public Camera cam;
	[Tooltip("World space offset")]
	public Vector3 wsOffset;
	[Tooltip("Screen space offs et")]
	public Vector2 ssOffset;

	[Tooltip("Will be moved based on the position of the unit.")]
	public GameObject parent;

	public RectTransform hpRect;
	public Text hpText;

	[SerializeField, HideInInspector] float hpFullWidth;

	void LateUpdate() {
		parent.transform.position = cam.WorldToScreenPoint(transform.position + wsOffset).Add(ssOffset);
		hpRect.sizeDelta = new Vector2(unit.unitData.health.value / unit.unitData.health.other * hpFullWidth, hpRect.sizeDelta.y);
		hpText.text = $"{Mathf.Ceil(unit.unitData.health.value)}/{Mathf.Ceil(unit.unitData.health.other)}";
	}

	new void Awake() {
		if (!cam) cam = Camera.main;
		if (!cam) cam = FindObjectOfType<Camera>();
		hpFullWidth = hpRect.sizeDelta.x;
		base.Awake();
	}

}
