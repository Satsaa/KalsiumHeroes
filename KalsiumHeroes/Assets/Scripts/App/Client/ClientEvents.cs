#nullable enable annotations

using System;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using NativeWebSocket;
using System.Threading.Tasks;

public static class ClientEvents {

	public interface IHandlable {
		public void Handle();
	}

	[Serializable]
	public class GameCreate : ClientEvent {
		public string code;
	}

	[Serializable]
	public class GameJoin : ClientEvent {
		public string code;
		public Team team;
	}

	[Serializable]
	public class GameSpectate : ClientEvent {
		public string code;
	}

	[Serializable]
	public class GameDelete : ClientEvent {
		public string code;
	}

	[Serializable]
	public class GameDisconnect : ClientEvent {
		public string code;
		public string? message;
	}

	[Serializable]
	public class GameEventList : ClientEvent, IHandlable {
		public string code;
		public string[] types;
		public string[] jsons;

		async void IHandlable.Handle() {
			await Task.Delay(1000);
			for (int i = 0; i < types.Length; i++) {
				var type = types[i];
				var json = jsons[i];
				var ge = JsonUtility.FromJson(json, Event.eventTypes[type]) as GameEvent;
				Game.events.AddEvent(ge);
			}
		}
	}

	[Serializable]
	public class RequestEvents : ClientEvent {
		public string code;
	}

	[Serializable]
	public class Result : ClientEvent {
		public ResultType result;
		public string to;
		public string? message;

		public bool succeeded => result == ResultType.Success;
		public bool failed => result == ResultType.Fail;
		public bool errored => result == ResultType.Error;
		public bool timeouted => result == ResultType.Timeout;
	}

	[Serializable]
	public class GenerateCode : ClientEvent { }

	[Serializable]
	public class GenerateCodeResult : Result {
		public string code;
	}


	public enum ResultType {
		Timeout = -1,
		Success,
		Fail,
		Error,

	}

}

#nullable disable
