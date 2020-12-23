using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[System.Serializable]
public class Rounds {

	public List<Unit> units = new List<Unit>();

	public Unit current {
		get {
			var res = units.LastOrDefault();
			if (!res) {
				Gather();
				res = units.LastOrDefault();
			}
			return res;
		}
	}

	[field: SerializeField] public int round { get; private set; }

	void Sort() {
		units.Sort((a, b) => a.unitData.speed.value.CompareTo(b.unitData.speed.value));
	}

	public List<Unit> GetEstimatedOrder(int roundsAhead) {
		var units = Game.dataComponents.Get<Unit>().Where(v => v).ToList();
		units.Sort((a, b) => a.GetEstimatedSpeed(roundsAhead).CompareTo(b.GetEstimatedSpeed(roundsAhead)));
		return units.ToList();
	}

	void Gather() {
		units = Game.dataComponents.Get<Unit>().Where(v => v).ToList();
		Sort();
	}

	/// <summary> Whether unit's turn has already passed during this round. </summary>
	public bool HasFinishedTurn(Unit unit) {
		return !units.Contains(unit);
	}

	public void NextTurn() {
		units.RemoveAll(v => v == null);
		OnTurnEnds();
		if (units.Count <= 1) {
			if (!TryEndGame()) {
				round++;
				Gather();
				OnRoundStarts();
				OnTurnStarts();
			}
			return;
		}
		units.RemoveAt(units.Count - 1);
		OnTurnStarts();
	}

	public void OnGameStart() {
		if (!TryEndGame()) {
			round++;
			Gather();
			OnRoundStarts();
			OnTurnStarts();
		}
	}

	private bool TryEndGame() {
		if (Game.dataComponents.Get<Unit>().Count() <= 1) {
			Debug.Log($"Game end");
			foreach (var modifier in Game.dataComponents.Get<Modifier>()) modifier.OnGameEnd();
			Game.InvokeOnAfterEvent();
			return true;
		}
		return false;
	}

	private void OnRoundStarts() {
		Debug.Log($"Round start: {units.Count} Unit(s)");
		foreach (var modifier in Game.dataComponents.Get<Modifier>()) modifier.OnRoundStart();
		Game.InvokeOnAfterEvent();
	}

	private void OnTurnEnds() {
		Debug.Log($"Turn end: {current.name}");
		foreach (var modifier in current.modifiers.Get()) modifier.OnTurnEnd();
		Game.InvokeOnAfterEvent();
	}

	private void OnTurnStarts() {
		Debug.Log($"Turn start: {current.name}");
		foreach (var modifier in current.modifiers.Get()) modifier.OnTurnStart();
		foreach (var modifier in Game.dataComponents.Get<Modifier>()) modifier.OnTurnStart(current);
		Game.InvokeOnAfterEvent();
	}
}