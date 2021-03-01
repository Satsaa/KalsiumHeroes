
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
	public class GameConnect : ClientEvent {
		public string code;
		public int player;
	}

}
