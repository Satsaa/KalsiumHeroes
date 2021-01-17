
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public abstract class GameEvent {
	public int eventNum;
	public abstract EventHandler GetHandler();
}

[Serializable]
public class Events {

	[SerializeReference] public List<object> stack = new List<object>();
	public GameEvent first => (GameEvent)stack[0];

	public bool finished => eventHandler == null;

	[SerializeReference]
	private EventHandler eventHandler = null;

	public void Update() {
		if (eventHandler != null) {
			if (eventHandler.EventHasEnded()) {
				eventHandler = null;
				Game.onEvents.Execute<IOnAnimationEventEnd>(v => v.OnAnimationEventEnd());
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
					Game.onEvents.Execute<IOnAnimationEventStart>(v => v.OnAnimationEventStart(eventHandler));
				}
			} catch (Exception) {
				Game.onEvents.Execute<IOnAnimationEventEnd>(v => v.OnAnimationEventEnd());
				throw;
			} finally {
				stack.RemoveAt(0);
			}
		}
	}


	public static Dictionary<string, Type> events => _events ?? BuildEventTypes();
	private static Dictionary<string, Type> _events;

	private static Dictionary<string, Type> BuildEventTypes() {
		_events = new Dictionary<string, Type>();
		var nested = typeof(Events).GetNestedTypes().Where(t => t.BaseType == typeof(GameEvent));
		foreach (var type in nested) {
			_events.Add(type.Name, type);
		}
		return _events;
	}

	public void QueueEvent(GameEventPacket data) {
		var ge = (GameEvent)JsonUtility.FromJson(data.json, events[data.name]);
		stack.Add(ge);
	}


	// DO NOT CHANGE CLASS NAMES OF DEPLOYED GameEvents

	[Serializable]
	public class Ability : GameEvent {

		/// <summary> ability id. </summary>
		public string ability;
		/// <summary> Tile of caster. </summary>
		public Vector3Int unit;
		/// <summary> An ordered list of targeted tiles. </summary>
		public Vector3Int[] targets;

		public override EventHandler GetHandler() {
			Debug.Log($"{this.GetType().Name}: Called");
			var unit = Game.grid.tiles[this.unit].unit;
			var ability = unit.modifiers.Get<global::Ability>().First(a => a.data.identifier == this.ability);
			EventHandler<Ability> abilityHandler = ability.CreateEventHandler(this);
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
		}

		/// <summary> Spawned starter units. </summary>
		public SpawnInfo[] units;

		public override EventHandler GetHandler() {
			Debug.Log($"{this.GetType().Name}: Called");

			Game.readyCount++;

			switch (Game.readyCount) {
				case 1:
					foreach (var info in this.units) {
						var tile = Game.grid.tiles[info.position];
						var unitData = Game.library.GetData<UnitData>(info.unit);
						var go = MasterComponent.Instantiate(unitData, Game.instance.transform);
						go.SetActive(false);
					}
					break;
				case 2:
					break;
				default:
					throw new InvalidOperationException($"Too many readies! ({Game.readyCount})");
			}


			return null;
		}
	}


}