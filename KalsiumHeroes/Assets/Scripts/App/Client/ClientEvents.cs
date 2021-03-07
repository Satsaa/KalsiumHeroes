#nullable enable annotations

using System;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using NativeWebSocket;

public static class ClientEvents {

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
	public class Result : ClientEvent {
		public ResultType result;
		public string to;
		public string? message;
	}


	public enum ResultType {
		Timeout = -1,
		Success,
		Fail,
		Error,

	}

}
