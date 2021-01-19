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
		units.Sort((a, b) => a.data.speed.value.CompareTo(b.data.speed.value));
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
			using (var scope = new OnEvents.Scope()) Game.onEvents.ForEach<IOnGameEnd>(scope, v => v.OnGameEnd());
			return true;
		}
		return false;
	}

	private void OnRoundStarts() {
		using (var scope = new OnEvents.Scope()) Game.onEvents.ForEach<IOnRoundStart>(scope, v => v.OnRoundStart());
	}

	private void OnTurnEnds() {
		using (var scope = new OnEvents.Scope()) {
			current.onEvents.ForEach<IOnTurnEnd_Unit>(scope, v => v.OnTurnEnd());
			current.tile.onEvents.ForEach<IOnTurnEnd_Tile>(scope, v => v.OnTurnEnd(current));
			Game.onEvents.ForEach<IOnTurnEnd_Global>(scope, v => v.OnTurnEnd(current));
		}
	}

	private void OnTurnStarts() {
		using (var scope = new OnEvents.Scope()) {
			current.onEvents.ForEach<IOnTurnStart_Unit>(scope, v => v.OnTurnStart());
			current.tile.onEvents.ForEach<IOnTurnStart_Tile>(scope, v => v.OnTurnStart(current));
			Game.onEvents.ForEach<IOnTurnStart_Global>(scope, v => v.OnTurnStart(current));
		}
	}
}