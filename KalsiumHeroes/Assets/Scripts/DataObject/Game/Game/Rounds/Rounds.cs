using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using Serialization;

[KeepRefToken]
public class Rounds : ScriptableObject {

	public List<Unit> units = new();
	[SerializeField] private int index;

	private Unit unitOrNull => index >= units.Count ? null : units[index];
	public Unit unit {
		get {
			var res = unitOrNull;
			if (res == null) {
				Gather();
				res = unitOrNull;
			} else if (res.removed) {
				var old = res;
				do {
					index++;
					res = unitOrNull;
					if (res == null) {
						Gather();
						res = unitOrNull;
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

	[field: SerializeField] public int round { get; private set; } = -1;

	void Sort() {
		units.Sort((a, b) => b.speed.current.value.CompareTo(a.speed.current.value));
	}

	public List<Unit> GetEstimatedOrder(int roundsAhead) {
		if (roundsAhead < 0) {
			throw new IndexOutOfRangeException();
		} else if (roundsAhead == 0) {
			return this.units.ToList();
		}
		var units = Game.dataObjects.Get<Unit>().Where(v => v).ToList();
		units.Sort((a, b) => b.GetEstimatedSpeed(roundsAhead).CompareTo(a.GetEstimatedSpeed(roundsAhead)));
		return units.ToList();
	}

	void Gather() {
		index = 0;
		units = Game.dataObjects.Get<Unit>().Where(v => v).ToList();
		Sort();
	}

	/// <summary> Whether unit's turn has already passed during this round. </summary>
	public bool HasFinishedTurn(Unit unit) {
		for (int i = index; i < units.Count; i++) {
			if (unit == units[i]) return false;
		}
		return true;
	}

	public void NextTurn() {
		OnTurnEnds(unit);
		index++;
		while (unitOrNull != null && unitOrNull.removed) {
			index++;
		}
		if (index >= units.Count) {
			if (!TryEndGame()) {
				round++;
				Gather();
				OnRoundStarts(); // 4
				OnTurnStarts(unit);
			}
			return;
		}
		OnTurnStarts(unit);
	}

	public void OnGameStart() {
		using (var scope = new Hooks.Scope()) Game.hooks.ForEach<IOnCombatLog>(scope, v => v.OnCombatLog("Game start"));
		if (!TryEndGame()) {
			round++;
			Gather();
			OnRoundStarts();
			OnTurnStarts(unit);
		}
	}

	private bool TryEndGame() {
		if (Game.dataObjects.Get<Unit>().Count() <= 1) {
			using (var scope = new Hooks.Scope()) Game.hooks.ForEach<IOnCombatLog>(scope, v => v.OnCombatLog("Game end"));
			using (var scope = new Hooks.Scope()) Game.hooks.ForEach<IOnGameEnd>(scope, v => v.OnGameEnd());
			return true;
		}
		return false;
	}

	private static void OnRoundStarts() {
		using (var scope = new Hooks.Scope()) Game.hooks.ForEach<IOnCombatLog>(scope, v => v.OnCombatLog($"Round {Game.rounds.round} begins"));
		using (var scope = new Hooks.Scope()) Game.hooks.ForEach<IOnRoundStart>(scope, v => v.OnRoundStart()); // 5 7
	}

	private static void OnTurnEnds(Unit unit) {
		using (var scope = new Hooks.Scope()) {
			unit.hooks.ForEach<IOnTurnEnd_Unit>(scope, v => v.OnTurnEnd());
			unit.tile.hooks.ForEach<IOnTurnEnd_Tile>(scope, v => v.OnTurnEnd(unit));
			Game.hooks.ForEach<IOnTurnEnd_Game>(scope, v => v.OnTurnEnd(unit));
		}
	}

	private static void OnTurnStarts(Unit unit) {
		using (var scope = new Hooks.Scope()) Game.hooks.ForEach<IOnCombatLog>(scope, v => v.OnCombatLog($"Turn: {Lang.GetStr($"{unit.identifier}_DisplayName")} ({unit.team})"));
		using (var scope = new Hooks.Scope()) {
			unit.hooks.ForEach<IOnTurnStart_Unit>(scope, v => v.OnTurnStart());
			unit.tile.hooks.ForEach<IOnTurnStart_Tile>(scope, v => v.OnTurnStart(unit));
			Game.hooks.ForEach<IOnTurnStart_Game>(scope, v => v.OnTurnStart(unit));
		}
	}
}