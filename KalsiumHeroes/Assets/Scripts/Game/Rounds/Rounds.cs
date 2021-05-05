using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[DisallowMultipleComponent]
public class Rounds : MonoBehaviour {

	public List<Unit> units = new List<Unit>();

	public Unit current {
		get {
			var res = units.LastOrDefault();
			if (res == null) {
				Gather();
				res = units.LastOrDefault();
			} else if (res.removed) {
				var old = res;
				do {
					units.RemoveAt(units.Count - 1);
					res = units.LastOrDefault();
					if (res == null) {
						Gather();
						res = units.LastOrDefault();
						break;
					}
				} while (res.removed);
				OnTurnEnds(old);
				OnTurnStarts(res);
				return res;
			}
			return res;
		}
	}

	[field: SerializeField] public int round { get; private set; }

	void Sort() {
		units.Sort((a, b) => a.data.speed.value.CompareTo(b.data.speed.value));
	}

	public List<Unit> GetEstimatedOrder(int roundsAhead) {
		var units = Game.dataObjects.Get<Unit>().Where(v => v).ToList();
		units.Sort((a, b) => a.GetEstimatedSpeed(roundsAhead).CompareTo(b.GetEstimatedSpeed(roundsAhead)));
		return units.ToList();
	}

	void Gather() {
		units = Game.dataObjects.Get<Unit>().Where(v => v).ToList();
		Sort();
	}

	/// <summary> Whether unit's turn has already passed during this round. </summary>
	public bool HasFinishedTurn(Unit unit) {
		return !units.Contains(unit);
	}

	public void NextTurn() {
		units.RemoveAll(v => v == null);
		OnTurnEnds(current);
		if (units.Count <= 1) {
			if (!TryEndGame()) {
				round++;
				Gather();
				OnRoundStarts(current);
				OnTurnStarts(current);
			}
			return;
		}
		units.RemoveAt(units.Count - 1);
		OnTurnStarts(current);
	}

	public void OnGameStart() {
		if (!TryEndGame()) {
			round++;
			Gather();
			OnRoundStarts(current);
			OnTurnStarts(current);
		}
	}

	private bool TryEndGame() {
		if (Game.dataObjects.Get<Unit>().Count() <= 1) {
			using (var scope = new Hooks.Scope()) Game.hooks.ForEach<IOnGameEnd>(scope, v => v.OnGameEnd());
			return true;
		}
		return false;
	}

	private static void OnRoundStarts(Unit unit) {
		using (var scope = new Hooks.Scope()) Game.hooks.ForEach<IOnRoundStart>(scope, v => v.OnRoundStart());
	}

	private static void OnTurnEnds(Unit unit) {
		using (var scope = new Hooks.Scope()) {
			unit.hooks.ForEach<IOnTurnEnd_Unit>(scope, v => v.OnTurnEnd());
			unit.tile.hooks.ForEach<IOnTurnEnd_Tile>(scope, v => v.OnTurnEnd(unit));
			Game.hooks.ForEach<IOnTurnEnd_Global>(scope, v => v.OnTurnEnd(unit));
		}
	}

	private static void OnTurnStarts(Unit unit) {
		using (var scope = new Hooks.Scope()) {
			unit.hooks.ForEach<IOnTurnStart_Unit>(scope, v => v.OnTurnStart());
			unit.tile.hooks.ForEach<IOnTurnStart_Tile>(scope, v => v.OnTurnStart(unit));
			Game.hooks.ForEach<IOnTurnStart_Global>(scope, v => v.OnTurnStart(unit));
		}
	}
}