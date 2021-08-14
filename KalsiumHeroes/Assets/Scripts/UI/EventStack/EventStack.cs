
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Extensions;
using UnityEngine.Pool;

public class EventStack : Muc.Components.VirtualLayoutGroup, IOnAnimationEventEnd, IOnRoundStart, IOnGameStart {

	[Serializable]
	protected class Tracker {
		public int round;
		public int index;
		public List<Unit> units;
		public bool updated;
	}

	public static EventStack eventStack => _instance;
	private static EventStack _instance;

	[Min(1)]
	public int futureRounds = 5;
	[SerializeField] protected RectTransform shade;
	[SerializeField] protected RectTransform reticle;
	[SerializeField] protected RectTransform roundPrefab;
	[SerializeField] protected RectTransform unitPrefab;

	[SerializeField] protected List<Tracker> trackers;

#if UNITY_EDITOR
	protected override void OnValidate() {
		base.OnValidate();
#else
	protected virtual void OnValidate() {
#endif
#if UNITY_EDITOR // Prevent activation in prefabs
		if (UnityEditor.SceneManagement.PrefabStageUtility.GetPrefabStage(gameObject) == null && !UnityEditor.PrefabUtility.IsPartOfPrefabAsset(gameObject))
#endif
			if (_instance != null && _instance != this) {
				Debug.LogWarning($"Multiple {typeof(EventStack).Name} GameObjects!", this);
				Debug.LogWarning($"Main instance of {typeof(EventStack).Name}: {_instance}", _instance);
			} else {
				_instance = this as EventStack;
			}
	}

	protected override void Awake() {
		base.Awake();
		if (_instance != null && _instance != this) {
			Debug.LogWarning($"Multiple {typeof(EventStack).Name} GameObjects!", this);
			Debug.LogWarning($"Main instance of {typeof(EventStack).Name}: {_instance}", _instance);
		} else {
			_instance = this as EventStack;
		}
		Game.hooks.Hook(this);
	}

	protected override void OnDestroy() {
		base.OnDestroy();
		if (_instance == this) {
			_instance = null;
		}
		if (Game.game) Game.hooks.Unhook(this);
	}

	public void MoveReticle(Unit unit) {
		var current = trackers[Game.rounds.round];
		var unitIndex = 0;
		for (int i = 0; i < current.units.Count; i++) {
			var other = current.units[i];
			if (other == unit) {
				unitIndex = current.index + i;
				break;
			}
		}
		var pos = 0f;
		for (int i = 0; i <= unitIndex; i++) {
			var item = items[i];
			pos += item.size;
		}
		pos -= items[unitIndex].size / 2;
		SetPos(reticle, pos);

	}

	protected void CreateRound() {
		var round = trackers.Any() ? trackers.Last().round + 1 : 0;

		var roundItem = AnimatedItem.CreateInstance<AnimatedItem>();
		roundItem.prefab = roundPrefab;
		roundItem.calculateSize = true;
		roundItem.createOnInit = true;
		base.Add(roundItem);
		ValueReceiver.SendValue(roundItem.item.gameObject, round);

		var units = Game.rounds.GetEstimatedOrder(round - Game.rounds.round);

		trackers.Add(new() {
			round = round,
			index = items.Count,
			units = units,
		});

		foreach (var unit in units) {
			var unitItem = AnimatedItem.CreateInstance<AnimatedItem>();
			unitItem.prefab = unitPrefab;
			unitItem.calculateSize = true;
			unitItem.createOnInit = true;
			base.Add(unitItem);
			ValueReceiver.SendValue(unitItem.item.gameObject, unit);
			ValueReceiver.SendValue(unitItem.item.gameObject, round);
		}

	}

	protected class ToFrom {
		public int from;
		public int to;
		public ToFrom(int from, int to) {
			this.from = from;
			this.to = to;
		}
		public override string ToString() {
			return $"{from}, {to}";
		}
	}

	public void OnAnimationEventEnd() {
		for (int i = Game.rounds.round + 1; i < trackers.Count; i++) {
			var tracker = trackers[i];
			var oldUnits = tracker.units;
			var newUnits = Game.rounds.GetEstimatedOrder(i - Game.rounds.round);
			if (oldUnits.Count == newUnits.Count && oldUnits.SequenceEqual(newUnits)) {
				continue;
			}

			var removed = ListPool<int>.Get();
			for (int j = 0; j < oldUnits.Count; j++) {
				var oldUnit = oldUnits[j];
				if (!newUnits.Contains(oldUnit)) removed.Add(j);
			}
			for (int j = removed.Count - 1; j >= 0; j--) {
				var removeAt = removed[j];
				base.RemoveAt(tracker.index + removeAt);
				oldUnits.RemoveAt(removeAt);
			}
			if (removed.Any()) {
				for (int j = i + 1; j < trackers.Count; j++) {
					var other = trackers[j].index -= removed.Count;
				}
			}
			ListPool<int>.Release(removed);

			var moves = ListPool<ToFrom>.Get();
			for (int j = 0; j < oldUnits.Count; j++) {
				moves.Add(new ToFrom(j, newUnits.IndexOf(oldUnits[j])));
			}
			moves.Sort((a, b) => a.to - b.to);
			for (int j = 0; j < moves.Count; j++) {
				var move = moves[j];
				if (move.from != move.to) {
					base.MoveItem(tracker.index + move.from, tracker.index + move.to);
					for (int k = j + 1; k < moves.Count; k++) {
						var other = moves[k];
						if (other.from <= move.from) {
							other.from++;
						}
					}
				}
			}
			ListPool<ToFrom>.Release(moves);

			tracker.units = newUnits;

		}
	}

	public void OnRoundStart() {
		if (Game.rounds.round > 0) {
			var tracker = trackers[Game.rounds.round - 1];
			var lastIndex = tracker.index + tracker.units.Count - 1;
			var pos = 0f;
			for (int i = 0; i <= lastIndex; i++) {
				var item = items[i];
				pos += item.size;
			}
			shade.sizeDelta = shade.sizeDelta.SetX(pos);
		}
		CreateRound();
	}

	public void OnGameStart() {
		for (int i = 0; i < futureRounds; i++) {
			CreateRound();
		}
	}

	public override void Add(Item item) => throw new InvalidOperationException("Not allowed");
	public override void Insert(int index, Item item) => throw new InvalidOperationException("Not allowed");
	public override void MoveItem(int from, int to) => throw new InvalidOperationException("Not allowed");
	public override void RemoveAt(int index) => throw new InvalidOperationException("Not allowed");

}