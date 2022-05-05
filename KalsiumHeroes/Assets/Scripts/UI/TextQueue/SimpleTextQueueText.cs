using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public class SimpleTextQueueText : TextQueueText {

	public override void OnShow(string text) {
		this.text = text;
		this.gameObject.SetActive(true);
	}

	public override void OnFadeIn(float t, Vector3 p, float a) {
		this.rectTransform.localPosition = p;
		this.alpha = a;
	}

	public override void OnLingerStart(bool instantFadeOut) {
		this.alpha = 1;
		this.rectTransform.localPosition = default;
	}

	public override void OnLinger(float t) {
	
	}

	public override void OnFadeOutStart() {
	
	}

	public override void OnFadeOut(float t, Vector3 p, float a) {
		this.rectTransform.localPosition = p;
		this.alpha = a;
	}

	public override void OnHide(bool pooled) {
		if (pooled) {
			this.gameObject.SetActive(false);
		} else {
			Destroy(this.gameObject);
		}
	}
}
