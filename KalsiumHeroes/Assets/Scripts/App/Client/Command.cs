
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Command {
	public Command(Event data) {
		this.command = data.command;
		this.data = data;
	}
	public string command;
	public Event data;
}

[Serializable]
public class Command<T> where T : Event {
	public Command(T data) {
		this.command = data.command;
		this.data = data;
	}
	public string command;
	public T data;
}
