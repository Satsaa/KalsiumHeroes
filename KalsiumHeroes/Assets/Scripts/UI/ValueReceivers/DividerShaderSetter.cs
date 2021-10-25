
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Extensions;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
public class DividerShaderSetter : ValueHooker<Unit>, IOnAnimationEventEnd {

	[SerializeField] NumericAttributeSelector attribute;

	[SerializeField, Tooltip("Set property named \"MaxValue\" to the maximum value of the attribute.")]
	bool setMaxValue = true;

	[SerializeField, Tooltip("Scale the property named \"WidthScale\" with .")]
	bool setWidthScale = true;

	[SerializeField, HideInInspector]
	float referenceWidth = -1;
	[SerializeField, HideInInspector]
	float referenceWidthScale = -1;
	[SerializeField, HideInInspector]
	bool awoken = false;
	[SerializeField, HideInInspector]
	Graphic graphic;

	protected void Awake() {
		awoken = true;
		graphic = GetComponent<Graphic>();
		if (setWidthScale) {
			var currentWidth = ((RectTransform)transform).rect.width;

			if (referenceWidth == -1) referenceWidth = currentWidth;
			if (referenceWidthScale == -1) referenceWidthScale = graphic.material.GetFloat("WidthScale");

			graphic.material = new Material(graphic.material); // We must clone the material to not share properties.

			graphic.material.SetFloat("WidthScale", currentWidth / referenceWidth * referenceWidthScale);
		}
	}

	// Called when dimensions of a RectTransform change
	protected void OnRectTransformDimensionsChange() {
		if (setWidthScale && awoken && Application.isPlaying) {
			var currentWidth = ((RectTransform)transform).rect.width;
			graphic.material.SetFloat("WidthScale", currentWidth / referenceWidth * referenceWidthScale);
		}
	}

	protected override void ReceiveValue(Unit target) {
		this.target = target;
		UpdateValue(target);
		Hook(target);
	}


	[SerializeField, HideInInspector]
	protected Unit target;

	protected void UpdateValue(Unit data) {

		if (data && (setMaxValue || setWidthScale)) {

			var current = attribute.GetValue(data);
			var maxValue = attribute.GetOther(data);
			maxValue = Mathf.Max(current, maxValue);

			if (setMaxValue) graphic.material.SetFloat("MaxValue", maxValue);
			if (setWidthScale) {
				var currentWidth = ((RectTransform)transform).rect.width;
				graphic.material.SetFloat("WidthScale", currentWidth / referenceWidth * referenceWidthScale);
			}

		}

	}

	public void OnAnimationEventEnd() => UpdateValue(target);

}