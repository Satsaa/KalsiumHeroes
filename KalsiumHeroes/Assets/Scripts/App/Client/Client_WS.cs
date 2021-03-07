
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using NativeWebSocket;

public partial class Client : MonoBehaviour {

	public WebSocket ws;
	public string url = "ws://localhost:8080";

	// Start is called before the first frame update
	async void Start() {
		ws = new WebSocket(url);

		ws.OnOpen += async () => {
			Debug.Log("Connection open!");
			await App.client.Post(new ClientEvents.GameCreate() { code = "TEST" });
			var join = await App.client.Post(new ClientEvents.GameJoin() { code = "TEST", team = Team.Team1 });
			if (join.result == ClientEvents.ResultType.Fail) { // Try other team
				await App.client.Post(new ClientEvents.GameJoin() { code = "TEST", team = Team.Team2 });
			}
		};

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

	void Update_WS() {
#if !UNITY_WEBGL || UNITY_EDITOR
		ws.DispatchMessageQueue();
#endif
	}

	async void OnApplicationQuit() {
		await ws.Close();
	}
}
