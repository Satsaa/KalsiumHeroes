using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TMProColorSwitcher : MonoBehaviour {

	public TextMeshProUGUI text;
	public Button button;

	public AnimationCurve transitionCurve;
	public float transitionDuration = 0.25f;

	public Color normalColor;
	public Color hoverColor;
	public Color pressColor;
	public Color disabledColor;

	Color prevTarget;
	public Color targetColor =>
		interactable
			? pressing
				? pressColor
				: hovering
					? hoverColor
					: normalColor
			: disabledColor;

	bool hovering;
	bool pressing;
	bool interactable => button ? button.interactable : false;

	Color fromColor;
	bool animating;
	float animStart;
	float t => (Time.time - animStart) / transitionDuration;

	void Reset() {
		text = GetComponentInChildren<TextMeshProUGUI>();
		button = transform.GetComponentInParent<Button>();
		prevTarget = targetColor;
	}

	void Start() {
		if (!text) text = GetComponentInChildren<TextMeshProUGUI>();
		if (!button) button = transform.parent.GetComponentInParent<Button>();
		fromColor = text.color;
	}

	void Update() {
		if (prevTarget != (prevTarget = targetColor)) {
			fromColor = text.color;
			animStart = Time.time;
			animating = fromColor != targetColor;
		}
		if (animating) {
			var t = this.t;
			if (t >= 1) {
				text.color = targetColor;
				animating = false;
			} else {
				var v = transitionCurve.Evaluate(t);
				var color = Color.LerpUnclamped(fromColor, targetColor, v);
				text.color = color;
			}
		}
	}

	public void OnPointerEnter() => hovering = true;
	public void OnPointerExit() => hovering = false;

	public void OnPointerDown() => pressing = true;
	public void OnPointerUp() => pressing = false;
}
