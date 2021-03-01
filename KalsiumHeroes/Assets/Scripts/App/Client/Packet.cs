
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Packet {

	public static Dictionary<string, Type> types { get; } = Assembly.GetAssembly(typeof(Packet)).GetTypes().Where(t => t.IsSubclassOf(typeof(Packet))).ToDictionary(v => v.Name, v => v);

	[field: SerializeField] public string type { get; protected set; }

	protected Packet() { }

	public Packet(Type type) {
		this.type = type.Name;
	}
}

public class EventPacket : Packet {

	public EventPacket() {
		this.type = this.GetType().Name;
	}

}

public abstract class GameEvent : EventPacket {
	public int gameEventNum;
	public abstract EventHandler GetHandler();
}

public class ClientEventPacket : Packet {

	public int gameEventNum;

	public ClientEventPacket() {
		this.type = this.GetType().Name;
	}

}

public abstract class ClientEvent : ClientEventPacket {

	public abstract EventHandler GetHandler();
}
