
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using NativeWebSocket;

public partial class Client : MonoBehaviour {

	public WebSocket ws;
	public string url = "ws://localhost:8080";

	private void Start() {
		Connect();
	}

	private async void Connect() {
		ws = new WebSocket(url);

		ws.OnOpen += () => Debug.Log("Connection open!");
		ws.OnError += (e) => Debug.Log($"Error! {e}");
		ws.OnClose += (e) => Debug.Log("Connection closed!");

		ws.OnMessage += (bytes) => {
			var message = System.Text.Encoding.UTF8.GetString(bytes);
			// Debug.Log("OnMessage! " + message);
			TryReceive(message);
		};

		// waiting for messages
		await ws.Connect();
	}

	private void Update_WS() {
#if !UNITY_WEBGL || UNITY_EDITOR
		if (ws == null) {
			Connect();
		} else {
			ws.DispatchMessageQueue();
		}
#endif
	}

	private async void OnApplicationQuit() {
		await ws.Close();
	}

#if UNITY_EDITOR
	[UnityEditor.Callbacks.DidReloadScripts]
	private static void OnReloadScripts() {
		// Rejoin old game
		if (Application.isPlaying) {
			App.client.Connect();
		}
	}
#endif
}
