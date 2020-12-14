
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

	/// <summary> When an event handler has ended. </summary>
	public event Action onFinish;
	/// <summary> When an event handler is created and starts. </summary>
	public event Action onStart;
	public bool finished => eventHandler == null;

	[SerializeReference]
	private EventHandler eventHandler = null;

	public void Update() {
		if (eventHandler != null) {
			if (eventHandler.EventHasEnded()) {
				eventHandler = null;
				onFinish?.Invoke();
			} else {
				eventHandler.Update();
				return;
			}
		}
		if (stack.Count > 0) {
			try {
				eventHandler = first.GetHandler();
				if (eventHandler != null) onStart?.Invoke();
			} catch (Exception) {
				onFinish?.Invoke();
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

		public string ability;
		public Vector3Int unit;
		public Vector3Int target;

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

}