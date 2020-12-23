
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Muc.Extensions;
using System;
using UnityEngine.UI;

[RequireComponent(typeof(EventStackItem))]
public class RoundStack : Modifier {

	public EventStackItem item;
	public RectTransform bgImageRect;
	public List<UnitStackItem> stack;

	public GameObject itemPrefab;

	public AnimationCurve expandCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
	public float expandTime = 1;
	public float speed = 1;

	public int round;

	bool isCurrent => Game.rounds.round == round;
	int roundsAhead => round - Game.rounds.round;

	new void Awake() {
		base.Awake();
		item = GetComponent<EventStackItem>();
	}
	void Start() {
		DoNonCurrent();
	}

	public void DoNonCurrent() {
		var units = Game.rounds.GetEstimatedOrder(roundsAhead);
		var newUnits = units.Where(u => !stack.Any(s => s.unit == u)).ToList();
		var oldItems = stack.Where(v => !units.Contains(v.unit)).ToList();
		foreach (var newUnit in newUnits) {
			var go = GameObject.Instantiate(itemPrefab, transform);
			var comp = go.GetComponent<UnitStackItem>();
			comp.Init(newUnit);
			stack.Add(comp);
		}
		foreach (var oldItem in oldItems) {
			stack.Remove(oldItem);
			Destroy(oldItem.gameObject);
		}
		stack.Sort((a, b) => units.IndexOf(b.unit).CompareTo(units.IndexOf(a.unit)));
	}

	public void DoCurrent() {
		var oldItems = stack.Where(v => !Game.rounds.units.Contains(v.unit)).ToList();
		foreach (var oldItem in oldItems) {
			stack.Remove(oldItem);
			Destroy(oldItem.gameObject);
		}
	}

	public void Refresh() {
		if (Game.rounds.round > round) { Destroy(gameObject); return; }
		if (isCurrent) DoCurrent();
		else DoNonCurrent();
	}

	public override void OnEventEnd() => Refresh();
	public override void OnTurnStart(Unit unit) => Refresh();

	public override void OnRoundStart() => Refresh();


	// | Mostly same as EventStack but with different item type
	// V

	protected virtual void Update() {
		var currentPos = 0f;
		stack.RemoveAll(v => !v);
		var maxX = 0f;
		for (int i = 0; i < stack.Count; i++) {
			var item = stack[i];
			item.time += Time.deltaTime;
			item.canvas.sortingOrder = -i;
			var rt = item.transform as RectTransform;
			rt.localScale = rt.localScale.SetX(expandCurve.Evaluate(Mathf.Clamp01(item.time / expandTime)));
			var width = item.fullWidth;
			item.targetPosition = currentPos;
			var sign = Mathf.Sign(rt.localPosition.x - item.targetPosition);
			item.velocity -= speed * sign * Time.deltaTime;
			if (Mathf.Sign(item.velocity) == sign) item.velocity = 0;
			rt.localPosition += new Vector3(item.velocity * Time.deltaTime, 0, 0);
			if (sign != Mathf.Sign(rt.localPosition.x - item.targetPosition)) {
				if (i == 0) item.OnReachZero(this);
				item.velocity = 0;
				rt.localPosition = rt.localPosition.SetX(item.targetPosition);
			}
			currentPos += width;
			maxX = Mathf.Max(maxX, rt.localPosition.x + rt.rect.max.x);
		}
		item.fullWidth = (int)currentPos;
		if (bgImageRect) bgImageRect.sizeDelta = bgImageRect.sizeDelta.SetX(maxX);
	}

	public void Add(UnitStackItem item) {
		item.transform.SetParent(transform);
		var pos = 0f;
		var last = stack.LastOrDefault();
		if (last != null) pos = last.transform.localPosition.x + last.fullWidth;
		item.transform.localPosition = item.transform.localPosition.SetX(pos);
		item.time = 0;
		stack.Add(item);
	}

	public void Insert(UnitStackItem item, int index) {
		var pos = 0f;
		var clamped = Mathf.Clamp(index, 0, stack.Count - 1);
		if (clamped > 0) {
			var prev = stack[index - 1];
			if (prev != null) pos = prev.transform.localPosition.x + prev.fullWidth;
		}
		item.transform.localPosition = item.transform.localPosition.SetX(pos);
		item.time = 0;
		stack.Insert(index, item);
	}

	public void Reposition(int oldIndex, int newIndex) {
		var item = stack[oldIndex];
		stack.RemoveAt(oldIndex);
		stack.Insert(newIndex, item);
	}

}
