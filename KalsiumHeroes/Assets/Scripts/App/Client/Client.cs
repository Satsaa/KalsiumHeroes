
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using NativeWebSocket;

[DisallowMultipleComponent]
public partial class Client : MonoBehaviour {

	public int globalEventNum = 0;

	public void Post(Event e) {
		switch (e) {
			case GameEvent gameEvent: {
					gameEvent.gameEventNum = Game.instance.gameEventNum++;
					gameEvent.code = "TEST";
					var targetType = typeof(Command<>).MakeGenericType(e.GetType());
					var command = Activator.CreateInstance(targetType, new object[] { gameEvent });
					ws.SendText(JsonUtility.ToJson(command));
				}
				break;
			case ClientEvent clientEvent: {
					var targetType = typeof(Command<>).MakeGenericType(e.GetType());
					var command = Activator.CreateInstance(targetType, new object[] { clientEvent });
					ws.SendText(JsonUtility.ToJson(command));
				}
				break;
			default:
				Debug.LogWarning($"Unspecialized outgoing {nameof(Event)}: {e.GetType().Name}", this);
				break;
		}
	}

	private void TryReceive(string json) {
		var packet = JsonUtility.FromJson<Command>(json);
		var targetType = typeof(Command<>).MakeGenericType(Event.types[packet.data.type]);
		Event e = (JsonUtility.FromJson(json, targetType) as dynamic).data;
		switch (e) {
			case GameEvent gameEvent:
				Game.events.QueueEvent(gameEvent);
				break;
			case ClientEvent clientEvent:
				Debug.Log($"Received {nameof(ClientEvent)}");
				break;
			default:
				Debug.LogWarning($"Unspecialized incoming {nameof(Event)}: {e.GetType().Name}", this);
				break;
		}
	}

}
