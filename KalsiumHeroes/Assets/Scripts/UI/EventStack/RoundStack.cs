
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Muc.Extensions;
using System;
using UnityEngine.UI;

[RequireComponent(typeof(StackItem))]
public class RoundStack : EventStack<RoundStackItem>, IOnAnimationEventEnd, IOnTurnStart_Global, IOnRoundStart {

	[HideInInspector] public StackItem thisItem;
	public RectTransform bgImageRect;
	public GameObject itemPrefab;

	public int round;

	bool isOld => Game.rounds.round > round;
	bool isCurrent => Game.rounds.round == round;
	bool isUpcoming => Game.rounds.round < round;
	int roundsAhead => round - Game.rounds.round;

	protected new void Awake() {
		base.Awake();
		thisItem = GetComponent<StackItem>();
		Game.onEvents.Add(this);
	}

	void Start() {
		DoNonCurrent();
	}

	protected new void Update() {
		base.Update();
		bgImageRect.sizeDelta = new Vector2(MaxX(), bgImageRect.sizeDelta.y);
	}

	void OnDestroy() {
		if (Game.game) Game.onEvents.Remove(this);
	}

	public void OnAnimationEventEnd() => Refresh();
	public void OnTurnStart(Unit unit) => Refresh();
	public void OnRoundStart() => Refresh();

	public void DoNonCurrent() {
		var units = Game.rounds.GetEstimatedOrder(roundsAhead);

		var newUnits = units.Where(u => !stack.Any(s => s.item.unit == u)).ToList();
		var oldItems = stack.Where(v => !units.Contains(v.item.unit)).ToList();

		foreach (var newUnit in newUnits) {
			var go = ObjectUtil.Instantiate(itemPrefab, transform);
			var comp = go.GetComponent<RoundStackItem>();
			comp.unit = newUnit;
			base.Add(comp);
		}

		foreach (var oldItem in oldItems) {
			oldItem.item.StartRemove();
		}
		_stack.Sort((a, b) => units.IndexOf(b.item.unit).CompareTo(units.IndexOf(a.item.unit)));
	}

	public void DoCurrent() {
		var oldItems = stack.Where(v => !Game.rounds.units.Contains(v.item.unit)).ToList();
		foreach (var oldItem in oldItems) {
			oldItem.item.StartRemove();
		}
	}

	public void Refresh() {
		if (Game.rounds.round > round) { Destroy(gameObject); return; }
		if (isCurrent) DoCurrent();
		else DoNonCurrent();
	}

}
