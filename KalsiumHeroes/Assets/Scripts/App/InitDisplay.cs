using System.Collections;
using System.Collections.Generic;
using Muc.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitDisplay : MonoBehaviour {

	public AsyncOperation op;

	public RectTransform barRect;
	public UnityEngine.Object destroyTarget;

	[SerializeField, HideInInspector] private float barFullWidth;

	protected void Awake() {
		this.barFullWidth = barRect.sizeDelta.x;
	}

	protected void Start() {
		barRect.sizeDelta = barRect.sizeDelta.SetX(0);
	}

	protected void Update() {
		if (op != null) {
			barRect.sizeDelta = barRect.sizeDelta.SetX(op.progress);
			if (op.isDone) {
				Destroy(this);
			}
		}
	}

	protected void OnDestroy() {
		if (destroyTarget != null) Destroy(destroyTarget);
	}
}