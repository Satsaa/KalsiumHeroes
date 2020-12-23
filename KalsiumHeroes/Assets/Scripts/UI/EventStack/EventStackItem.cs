
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas), typeof(CanvasGroup))]
public class EventStackItem : MonoBehaviour {

	public Canvas canvas;
	public CanvasGroup group;

	public int fullWidth = 100;

	public float time = 0;
	public float velocity = 0;
	public float targetPosition = 0;

	void Awake() {
		canvas = GetComponent<Canvas>();
		group = GetComponent<CanvasGroup>();
	}

	/// <summary>
	/// When this item is first in the stack and reached target position
	/// </summary>
	public virtual void OnReachZero(EventStack es) { }

}