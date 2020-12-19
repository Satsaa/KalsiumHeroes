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
		units = Game.dataComponents.Get<Unit>().ToList();
		Sort();
	}

	public void NextTurn() {
		units.RemoveAll(v => v == null);
		OnTurnEnds();
		if (units.Count <= 1) {
			round++;
			Gather();
			OnRoundStarts();
			OnTurnStarts();
			return;
		}
		units.RemoveAt(units.Count - 1);
		OnTurnStarts();
	}

	public void OnGameStart() {
		round++;
		Gather();
		OnRoundStarts();
		OnTurnStarts();
	}

	private void OnRoundStarts() {
		var _ = current;
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