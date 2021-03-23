using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class TMProColorSwitcher : MonoBehaviour,
IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler {

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

	void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) => hovering = true;
	void IPointerExitHandler.OnPointerExit(PointerEventData eventData) => hovering = false;
	void IPointerDownHandler.OnPointerDown(PointerEventData eventData) => pressing = true;
	void IPointerUpHandler.OnPointerUp(PointerEventData eventData) => pressing = false;
}
