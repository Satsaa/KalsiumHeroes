
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class Client : MonoBehaviour {

	public int globalEventNum = 0;

	public void PostEvent(GameEvent e) {
		e.gameEventNum = Game.instance.gameEventNum++;

		var packet = new GameEventPacket(e.GetType().Name, e);
		var fullJson = JsonUtility.ToJson(packet);

		// Send to server

		ReceiveEvent(fullJson);
	}

	private void ReceiveEvent(string json) {
		var packet = JsonUtility.FromJson<GameEventPacket>(json);
		Game.events.QueueEvent(packet);
	}
}

public class GameEventPacket {
	public GameEventPacket(string name, GameEvent e) {
		this.name = name;
		this.json = JsonUtility.ToJson(e);
	}
	public GameEventPacket(string name, string json) {
		this.name = name;
		this.json = json;
	}
	public string name;
	public string json;
}
