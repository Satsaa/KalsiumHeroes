using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[System.Serializable]
public class Rounds {

	[SerializeField, HideInInspector] List<Unit> units = new List<Unit>();

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

	public void Sort() {
		units.Sort((a, b) => a.unitData.speed.value.CompareTo(b.unitData.speed.value));
	}

	void Gather() {
		units = Game.dataComponents.Get<Unit>().Where(v => v).ToList();
		Sort();
	}

	public void NextTurn() {
		units.RemoveAll(v => v == null);
		OnTurnEnds();
		if (units.Count <= 1) {
			round++;
			Gather();
			if (!TryEndGame()) {
				OnRoundStarts();
				OnTurnStarts();
			}
			return;
		}
		units.RemoveAt(units.Count - 1);
		OnTurnStarts();
	}

	public void OnGameStart() {
		round++;
		Gather();
		if (!TryEndGame()) {
			OnRoundStarts();
			OnTurnStarts();
		}
	}

	private bool TryEndGame() {
		if (Game.dataComponents.Get<Unit>().Count() <= 1) {
			Debug.Log($"Game end");
			foreach (var modifier in Game.dataComponents.Get<Modifier>()) modifier.OnGameEnd();
			return true;
		}
		return false;
	}

	private void OnRoundStarts() {
		Debug.Log($"Round start: {units.Count} Unit(s)");
		foreach (var modifier in Game.dataComponents.Get<Modifier>()) modifier.OnRoundStart();
	}
	private void OnTurnEnds() {
		Debug.Log($"Turn end: {current.name}");
		foreach (var modifier in current.modifiers.Get()) modifier.OnTurnEnd();
	}
	private void OnTurnStarts() {
		Debug.Log($"Turn start: {current.name}");
		foreach (var modifier in current.modifiers.Get()) modifier.OnTurnStart();
		foreach (var modifier in Game.dataComponents.Get<Modifier>()) modifier.OnTurnStart(current);
	}
}