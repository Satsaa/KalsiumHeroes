
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Muc.Extensions;

[RequireComponent(typeof(Canvas), typeof(RectTransform))]
public class EventStackItem : MonoBehaviour {

	[Serializable]
	public class Repositioning {
		public float startTime;
		public float moved;
		public float offset;
		public Repositioning(float offset) {
			startTime = Time.time;
			this.offset = offset;
		}
	}

	[HideInInspector] public Canvas canvas;
	[HideInInspector] public RectTransform rt;

	public virtual float width => rt ? Mathf.RoundToInt(rt.rect.width) : 0;

	public AnimationCurve repositionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
	public float repositionDuration = 1;
	public List<Repositioning> repositionings;


	protected void Awake() {
		canvas = GetComponent<Canvas>();
		rt = GetComponent<RectTransform>();
		repositionings.Clear();
	}

	protected void Update() {
		if (!repositionings.Any()) return;
		List<Repositioning> finished = new List<Repositioning>();
		foreach (var r in repositionings) {
			var t = (Time.time - r.startTime) / repositionDuration;
			var v = (t >= 1 ? 1 : repositionCurve.Evaluate(t)) * r.offset;
			var move = v - r.moved;
			rt.localPosition += new Vector3(move, 0, 0);
			r.moved += move;
			if (t >= 1) finished.Add(r);
		}
		repositionings.RemoveAll(v => finished.Contains(v));
	}

	/// <summary>
	/// When this item is first in the stack and reached target position
	/// </summary>
	public virtual void OnReachZero(EventStack es) { }

	public virtual void StartReposition(float offset) {
		repositionings.Add(new Repositioning(offset));
	}

	public virtual void OnAdd() { }

	public virtual void StartRemove() {
		if (this != null) Destroy(gameObject);
	}

}