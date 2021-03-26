
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Collections;
using UnityEngine.EventSystems;
using Muc.Extensions;
using Muc.Time;

[RequireComponent(typeof(CanvasGroup))]

public class TooltipRoot : MonoBehaviour {

	[SerializeField] float duration = 0.25f;
	[SerializeField] AnimationCurve scaleCurve;
	[SerializeField] AnimationCurve alphaCurve;

	[HideInInspector, SerializeField] float t = 0f;
	[HideInInspector, SerializeField] int sign = 0;
	[HideInInspector, SerializeField] CanvasGroup group;

	protected virtual void Awake() {
		group = GetComponent<CanvasGroup>();
	}

	protected virtual void Update() {
		t += Time.deltaTime * (1f / duration) * sign;
		UpdateAnim();
	}

	protected void UpdateAnim() {
		if (t < 0 || t > 1) {
			sign = 0;
			enabled = false;
			t = Mathf.Clamp01(t);
			if (t == 0) {
				Destroy(gameObject);
			} else if (1 == 1) {
				group.interactable = true;
				group.blocksRaycasts = true;
			}
		}
		var scale = scaleCurve.Evaluate(t);
		var alpha = alphaCurve.Evaluate(t);
		transform.localScale = new Vector3(scale, scale, scale);
		group.alpha = alpha;
	}

	public virtual void Show() {
		sign = 1;
		enabled = true;
		group.interactable = false;
		group.blocksRaycasts = false;
		UpdateAnim();
	}

	public virtual void Hide() {
		sign = -1;
		enabled = true;
		group.interactable = false;
		group.blocksRaycasts = false;
		UpdateAnim();
	}

	public virtual void FinishAnims() {
		if (sign < 0) {
			t = 0;
		} else if (sign > 0) {
			t = 1;
		}
		t += Time.deltaTime * (1f / duration) * sign;
		UpdateAnim();
	}
}

