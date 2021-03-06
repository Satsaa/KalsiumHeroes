
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Event {

	public static Dictionary<string, Type> eventTypes { get; } = Assembly.GetAssembly(typeof(Event)).GetTypes().Where(t => t.IsSubclassOf(typeof(Event))).ToDictionary(v => v.Name, v => v);

	public string type;
	public virtual string command => this.GetType().Name;

	public Event() => this.type = this.GetType().Name;
}

[Serializable]
public abstract class GameEvent : Event {
	public string guid;
	public int gameEventNum;
	public string code;
	public abstract EventHandler GetHandler();
	public override string command => nameof(GameEvent);
}

[Serializable]
public abstract class ClientEvent : Event {
	public string guid;
}
