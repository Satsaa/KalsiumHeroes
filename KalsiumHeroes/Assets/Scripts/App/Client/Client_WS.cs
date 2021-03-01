
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using NativeWebSocket;

public partial class Client : MonoBehaviour {

	public WebSocket ws;

	// Start is called before the first frame update
	async void Start() {
		ws = new WebSocket("ws://localhost:8080");

		ws.OnOpen += () => {
			Debug.Log("Connection open!");
			App.client.Post(new ClientEvents.GameCreate() { code = "TEST" });
			App.client.Post(new ClientEvents.GameConnect() { code = "TEST", player = 0 });
		};

		ws.OnError += (e) => {
			Debug.Log($"Error! {e}");
		};

		ws.OnClose += (e) => {
			Debug.Log("Connection closed!");
		};

		ws.OnMessage += (bytes) => {
			var message = System.Text.Encoding.UTF8.GetString(bytes);
			Debug.Log("OnMessage! " + message);
			TryReceive(message);
		};

		// waiting for messages
		await ws.Connect();
	}

	void Update() {
#if !UNITY_WEBGL || UNITY_EDITOR
		ws.DispatchMessageQueue();
#endif
	}

	async void OnApplicationQuit() {
		await ws.Close();
	}
}
