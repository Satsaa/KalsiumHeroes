
using System;
using System.Reflection;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using static ClientEvents;
using Muc.Time;

[DisallowMultipleComponent]
public partial class Client : MonoBehaviour {

	public static Dictionary<string, SendEventTask> cets => _cets ??= new Dictionary<string, SendEventTask>();
	private static Dictionary<string, SendEventTask> _cets;

	void Update() {
		Update_WS();
		foreach (var cet in cets.Values) {
			if (cet.timeout.expired) {
				cet.tcs.TrySetCanceled();
			}
		}
	}

	public async Task<T> Post<T>(ClientEvent e) where T : Result {
		return await Post(e) as T;
	}

	public Task<Result> Post(ClientEvent e) {
		// Debug.Log(JsonUtility.ToJson(e));
		e.guid = Guid.NewGuid().ToString();
		var targetType = typeof(Command<>).MakeGenericType(e.GetType());
		var command = Activator.CreateInstance(targetType, new object[] { e });

		var res = new TaskCompletionSource<Result>();
		cets.Add(e.guid, new SendEventTask(new Timeout(5), res));
		ws.SendText(JsonUtility.ToJson(command));
		return res.Task;
	}

	public int gameEventsAwaiting = 0;

	public async void Post(GameEvent e) {
		// Debug.Log(JsonUtility.ToJson(e));
		if (Game.events.maxReceivedNum + gameEventsAwaiting != Game.events.receivedNum) {
			Debug.LogWarning("Didn't send event because we seem to be missing or awaiting pieces.");
			return;
		}
		e.gameEventNum = Game.events.maxReceivedNum + gameEventsAwaiting + 1;
		e.guid = Guid.NewGuid().ToString();
		e.code = Game.game.code;
		var targetType = typeof(Command<>).MakeGenericType(e.GetType());
		var command = Activator.CreateInstance(targetType, new object[] { e });

		gameEventsAwaiting++;
		var res = new TaskCompletionSource<Result>();
		cets.Add(e.guid, new SendEventTask(new Timeout(5), res));
		try {
			await ws.SendText(JsonUtility.ToJson(command));
			await res.Task;
		} finally {
			gameEventsAwaiting--;
		}
	}

	private void TryReceive(string json) {
		Debug.Log(json);
		var packet = JsonUtility.FromJson<Command>(json);
		if (Event.eventTypes.TryGetValue(packet.data.type, out var genericParam)) {

			var targetType = typeof(Command<>).MakeGenericType(genericParam);
			Event e = (JsonUtility.FromJson(json, targetType) as dynamic).data;
			switch (e) {
				case Result result:
					Debug.Log($"Received {nameof(Result)}: {json}");
					if (!String.IsNullOrWhiteSpace(result.to)) {
						if (cets.TryGetValue(result.to, out var tcs)) {
							tcs.tcs.SetResult(result);
							cets.Remove(result.to);
						} else {
							Debug.LogWarning($"Received result for unknown TaskCompletionSource: {json}");
						}
					}
					break;
				case GameEvent gameEvent:
					Game.events.AddEvent(gameEvent);
					break;
				case IHandlable handlable:
					handlable.Handle();
					break;
				case ClientEvent clientEvent:
					Debug.Log($"Received an unknown {nameof(ClientEvent)}: {clientEvent.type}");
					break;
				default:
					Debug.LogWarning($"Unspecialized incoming {nameof(Event)}: {e.GetType().Name}", this);
					break;
			}
		}
	}


	public class SendEventTask {

		public Timeout timeout;
		public TaskCompletionSource<Result> tcs;

		public SendEventTask(Timeout timeout, TaskCompletionSource<Result> tcs) {
			this.timeout = timeout;
			this.tcs = tcs;
		}
	}

}
