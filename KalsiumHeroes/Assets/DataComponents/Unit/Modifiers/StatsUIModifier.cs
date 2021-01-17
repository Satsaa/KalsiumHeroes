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

	public RectTransform energyRect;
	public Text energyText;

	[SerializeField, HideInInspector] float hpFullWidth;
	[SerializeField, HideInInspector] float energyFullWidth;

	void LateUpdate() {
		parent.transform.position = cam.WorldToScreenPoint(transform.position + wsOffset).Add(ssOffset);

		hpRect.sizeDelta = hpRect.sizeDelta.SetX(unit.data.health.value / unit.data.health.other * hpFullWidth);
		hpText.text = $"{Mathf.Ceil(unit.data.health.value)}/{Mathf.Ceil(unit.data.health.other)}";

		energyRect.sizeDelta = energyRect.sizeDelta.SetX(unit.data.energy.value / (float)unit.data.energy.other * energyFullWidth);
		energyText.text = $"{unit.data.energy.value}/{unit.data.energy.other}";
	}

	new void Awake() {
		base.Awake();
		if (!cam) cam = Camera.main;
		if (!cam) cam = FindObjectOfType<Camera>();
		hpFullWidth = hpRect.sizeDelta.x;
		energyFullWidth = energyRect.sizeDelta.x;
	}

}
