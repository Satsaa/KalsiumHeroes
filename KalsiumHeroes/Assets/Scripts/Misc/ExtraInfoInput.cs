
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Systems.Input;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Muc.Extensions;
using UnityEngine.InputSystem.Controls;

[RequireComponent(typeof(Graphic))]
public class ExtraInfoInput : ButtonInput {

	[SerializeField] float animDuration = 1;
	[SerializeField] AnimationCurve alphaMultCurve = new(new Keyframe(0, 0, 2, 2), new Keyframe(1, 1));
	[SerializeField] AnimationCurve scaleCurve = new(new Keyframe(0, 0, 2, 2), new Keyframe(1, 1));

	[SerializeField, HideInInspector] Graphic graphic;
	[SerializeField, HideInInspector] float baseAlpha;

	int sign;
	float t;

#if UNITY_EDITOR
	new void Reset() {
		base.Reset();
		UnityEditor.Events.UnityEventTools.AddPersistentListener(base.onStateChange, OnStateChange);
	}
#endif

	void Awake() {
		graphic = GetComponent<Graphic>();
		baseAlpha = graphic.color.a;
	}

	void Start() {
		var action = actionAsset.FindAction(actionId);
		if (action != null) {
			var pressPoint = InputSystem.settings.defaultButtonPressPoint;
			if (action.activeControl is ButtonControl bc) {
				pressPoint = bc.pressPointOrDefault;
			}
			var val = action.ReadValue<float>();
			var pressed = val >= pressPoint;
			graphic.enabled = pressed;
			if (pressed) {
				ShowExtraInfo();
			}
		}
	}

	void Update() {
		if (sign != 0) {
			t += sign * Time.deltaTime / animDuration;

			var scale = scaleCurve.Evaluate(t);
			transform.localScale = Vector3.one * scale;

			var alpha = alphaMultCurve.Evaluate(t) * this.baseAlpha;
			var color = graphic.color;
			color.a = alpha;
			graphic.color = color;

			if (t <= 0) {
				t = 0;
				sign = 0;
				graphic.enabled = false;
			} else if (t >= 1) {
				t = 1;
				sign = 0;
			}
		}

	}

	public void OnStateChange(bool state) {
		if (state) ShowExtraInfo();
		else HideExtraInfo();
	}

	public void ShowExtraInfo() {
		if (sign == 1 || t >= 1) return;
		sign = 1;
		t = Mathf.Max(t, 0);
		graphic.enabled = true;
	}

	public void HideExtraInfo() {
		if (sign == -1 || t <= 0) return;
		sign = -1;
		t = Mathf.Min(t, 1);
	}

}