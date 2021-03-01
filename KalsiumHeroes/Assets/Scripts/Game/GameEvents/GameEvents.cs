
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


[DisallowMultipleComponent]
public class GameEvents : MonoBehaviour {

	[SerializeReference] public List<object> stack = new List<object>();
	public GameEvent first => (GameEvent)stack[0];

	public bool finished => eventHandler == null;

	[SerializeReference]
	private EventHandler eventHandler = null;

	public void Update() {
		if (eventHandler != null) {
			if (eventHandler.EventHasEnded()) {
				eventHandler = null;
				using (var scope = new OnEvents.Scope()) Game.onEvents.ForEach<IOnAnimationEventEnd>(scope, v => v.OnAnimationEventEnd());
			} else {
				eventHandler.Update();
				return;
			}
		}
		if (stack.Count > 0) {
			try {
				eventHandler = first.GetHandler();
				if (eventHandler != null) {
					eventHandler.Update();
					using (var scope = new OnEvents.Scope()) Game.onEvents.ForEach<IOnAnimationEventStart>(scope, v => v.OnAnimationEventStart(eventHandler));
				}
			} catch (Exception) {
				using (var scope = new OnEvents.Scope()) Game.onEvents.ForEach<IOnAnimationEventEnd>(scope, v => v.OnAnimationEventEnd());
				throw;
			} finally {
				stack.RemoveAt(0);
			}
		}
	}


	public void QueueEvent(GameEvent gameEvent) {
		stack.Add(gameEvent);
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
			var unit = Game.grid.tiles[casterTile].units[casterIndex];
			var ability = unit.modifiers.Get<global::Ability>().ToList()[abilityIndex];
			EventHandler<Ability> abilityHandler = ability.CreateHandler(this);
			ability.OnCast();
			return abilityHandler;
		}
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

			/// <summary> The team of the unit. </summary>
			public Team team;
		}

		/// <summary> Spawned starter units. </summary>
		public SpawnInfo[] spawns;

		public override EventHandler GetHandler() {
			Debug.Log($"{this.GetType().Name}: Called");

			Game.instance.readyCount++;

			// TODO
			switch (Game.instance.readyCount) {
				case 1:
				case 2:
					foreach (var spawn in spawns) {
						var tile = Game.grid.tiles[spawn.position];
						var unitData = App.library.GetById<UnitData>(spawn.unit);
						var unit = Unit.Create(unitData, Game.grid.tiles[spawn.position], spawn.team);
						unit.actor.gameObject.SetActive(spawn.team == Game.instance.team);
					}
					if (Game.instance.readyCount == 2) {
						foreach (var unit in Game.dataObjects.Get<Unit>()) {
							unit.actor.gameObject.SetActive(true);
						}
						Game.instance.StartGame();
					}
					break;
				default:
					throw new InvalidOperationException($"Too many readies! ({Game.instance.readyCount})");
			}


			return null;
		}
	}


}