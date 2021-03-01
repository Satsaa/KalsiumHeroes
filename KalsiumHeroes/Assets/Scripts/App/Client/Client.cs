
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using NativeWebSocket;

[DisallowMultipleComponent]
public class Client : MonoBehaviour {

	public WebSocket ws;

	public int globalEventNum = 0;

	public void PostEvent(GameEvent e) {
		e.gameEventNum = Game.instance.gameEventNum++;

		var json = JsonUtility.ToJson(e);

		// Send to server

		Receive(json);
	}

	private void Receive(string json) {
		var packet = JsonUtility.FromJson<EventPacket>(json);
		var gameEvent = (GameEvent)JsonUtility.FromJson(json, Packet.types[packet.type]);
		Game.events.QueueEvent(gameEvent);
	}
}
