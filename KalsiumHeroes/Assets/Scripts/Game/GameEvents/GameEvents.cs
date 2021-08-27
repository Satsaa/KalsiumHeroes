
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Serialization;

[KeepRefToken]
public class GameEvents : ScriptableObject, IOnUpdate {

	[SerializeReference]
	public List<GameEvent> events = new List<GameEvent>();
	public GameEvent next => executedNum >= receivedNum ? null : events[executedNum + 1];
	public int maxQueued = 5;
	public int executedNum = -1;
	public int receivedNum = -1;
	public int maxReceivedNum => events.Count - 1;

	int fastForwardingUntil = -1;
	public bool fastForwarding => executedNum < fastForwardingUntil;

	public bool animating => handler != null;

	[field: SerializeReference]
	public EventHandler handler { get; private set; } = null;

	void OnValidate() {
		events?.Clear();
		executedNum = receivedNum = -1;
	}

	void IOnUpdate.OnUpdate() {
		if (handler != null) {
			if (handler.EventHasEnded()) {
				handler = null;
				using (var scope = new Hooks.Scope()) Game.hooks.ForEach<IOnAnimationEventEnd>(scope, v => v.OnAnimationEventEnd());
			} else {
				if (fastForwarding) {
					if (!handler.TryEnd()) {
						handler.Update();
					}
				} else {
					handler.Update();
				}
				return;
			}
		}
		if (next != null) {
			try {
				handler = next.GetHandler();
				executedNum++;
				if (handler != null) {
					handler.Update(); // 1
					using (var scope = new Hooks.Scope()) Game.hooks.ForEach<IOnAnimationEventStart>(scope, v => v.OnAnimationEventStart(handler));
				}
			} catch (Exception) {
				using (var scope = new Hooks.Scope()) Game.hooks.ForEach<IOnAnimationEventEnd>(scope, v => v.OnAnimationEventEnd());
				throw;
			}
		} else {
			fastForwardingUntil = -1;
		}
	}


	public void AddEvent(GameEvent gameEvent) {
		if (gameEvent.gameEventNum < 0 || gameEvent.gameEventNum > 100000) return;
		var addItems = gameEvent.gameEventNum - events.Count + 1;
		for (int i = 0; i < addItems; i++) events.Add(null);
		if (events[gameEvent.gameEventNum] != null) Debug.LogWarning("Replacing a GameEvent which shouldn't happen");
		events[gameEvent.gameEventNum] = gameEvent;
		while (receivedNum + 1 < events.Count) {
			if (events[receivedNum + 1] != null) receivedNum++;
		}
		if (receivedNum > executedNum + maxQueued) {
			FastForward();
		}
	}

	public bool TryEndEvent() {
		if (handler == null || handler.EventHasEnded()) return false;
		if (handler.TryEnd()) {
			return true;
		}
		return false;
	}

	public void FastForward(int maxEvents = -1) {
		fastForwardingUntil = maxEvents == -1 ? int.MaxValue : maxEvents;
	}

	// DO NOT CHANGE CLASS NAMES OF DEPLOYED GameEvents

	[Serializable]
	public class Ability : GameEvent {

		/// <summary> Tile of caster. </summary>
		public Vector3Int casterTile;
		/// <summary> Index of Unit. </summary>
		public int casterIndex;

		/// <summary> Index of casted Ability. </summary>
		public int abilityIndex;

		/// <summary> An ordered list of targeted Tiles. </summary>
		public Vector3Int[] targets;
		/// <summary> Indexes of target Tiles. </summary>
		public int[] targetIndexes;

		public override EventHandler GetHandler() {
			Debug.Log($"{this.GetType().Name}: Called");
			var caster = ResolveCaster();
			var ability = ResolveAbility(caster);
			var str = ability.CombatLog(this);
			if (str != null) using (var scope = new Hooks.Scope()) Game.hooks.ForEach<IOnCombatLog>(scope, v => v.OnCombatLog(str));
			EventHandler<Ability> abilityHandler = ability.CreateHandler(this);
			ability.OnCast();
			return abilityHandler;
		}

		public Unit ResolveCaster() => Game.grid.tiles[casterTile].units[casterIndex];
		public global::Ability ResolveAbility(Unit caster) => caster.modifiers.Get<global::Ability>().ToList()[abilityIndex];
	}

	[Serializable]
	public class Turn : GameEvent {

		public override EventHandler GetHandler() {
			Game.rounds.NextTurn();
			return null;
		}
	}

	[Serializable]
	public class Ready : GameEvent {

		[Serializable]
		public class SpawnInfo {

			/// <summary> unit id. </summary>
			public string unit;

			/// <summary> Tile to spawn unit at. </summary>
			public Vector3Int position;

		}

		/// <summary> The team of the units. </summary>
		public Team team;

		/// <summary> Spawned starter units. </summary>
		public SpawnInfo[] spawns;

		public override EventHandler GetHandler() {
			Debug.Log($"{this.GetType().Name}: Called");

			if (Game.game.started) {
				Debug.LogWarning("Readied after Game already started.");
				return null;
			}

			foreach (var spawn in spawns) {
				var tile = Game.grid.tiles[spawn.position];
				var unitData = App.library.GetById<UnitData>(spawn.unit);
				var unit = Unit.Create(unitData, Game.grid.tiles[spawn.position], 0, team);
				unit.actor.gameObject.SetActive(team == Game.game.team);
			}

			if (!Game.game.readied.Contains(team)) {
				Game.game.readied.Add(team);
			}

			if (Game.game.readied.Count == Game.game.mode.teams.Count) {
				foreach (var spawnCtrl in FindObjectsOfType<SpawnControl>()) {
					Destroy(spawnCtrl.gameObject);
				}
				foreach (var unit in Game.dataObjects.Get<Unit>()) {
					unit.actor.gameObject.SetActive(true);
				}

				Game.game.StartGame();
			}

			using (var scope = new Hooks.Scope()) Game.hooks.ForEach<IOnTeamReady>(scope, v => v.OnTeamReady(team));

			return null;
		}
	}


}