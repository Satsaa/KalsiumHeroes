
using System;
using System.Reflection;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using static ClientEvents;
using Muc.Time;

[DisallowMultipleComponent]
public partial class Client : MonoBehaviour {

	public static Dictionary<string, ClientEventTask> cets => _cets ??= new Dictionary<string, ClientEventTask>();
	private static Dictionary<string, ClientEventTask> _cets;

	void Update() {
		Update_WS();
		foreach (var cet in cets.Values) {
			if (cet.timeout.expired) {
				cet.tcs.TrySetCanceled();
			}
		}
	}

	public Task<Result> Post(ClientEvent e) {
		// Debug.Log(JsonUtility.ToJson(e));
		var res = new TaskCompletionSource<Result>();
		e.guid = Guid.NewGuid().ToString();
		cets.Add(e.guid, new ClientEventTask(new Timeout(5), res));
		var targetType = typeof(Command<>).MakeGenericType(e.GetType());
		var command = Activator.CreateInstance(targetType, new object[] { e });
		ws.SendText(JsonUtility.ToJson(command));
		return res.Task;
	}

	public void Post(GameEvent e) {
		// Debug.Log(JsonUtility.ToJson(e));
		e.gameEventNum = Game.game.gameEventNum++;
		e.code = Game.game.code;
		var targetType = typeof(Command<>).MakeGenericType(e.GetType());
		var command = Activator.CreateInstance(targetType, new object[] { e });
		ws.SendText(JsonUtility.ToJson(command));
	}

	private void TryReceive(string json) {
		Debug.Log(json);
		var packet = JsonUtility.FromJson<Command>(json);
		if (Event.types.TryGetValue(packet.data.type, out var genericParam)) {

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
				case ClientEvent clientEvent:
					Debug.Log($"Received {nameof(ClientEvent)}");
					break;
				case GameEvent gameEvent:
					Game.events.QueueEvent(gameEvent);
					break;
				default:
					Debug.LogWarning($"Unspecialized incoming {nameof(Event)}: {e.GetType().Name}", this);
					break;
			}
		}
	}


	public class ClientEventTask {

		public Timeout timeout;
		public TaskCompletionSource<Result> tcs;

		public ClientEventTask(Timeout timeout, TaskCompletionSource<Result> tcs) {
			this.timeout = timeout;
			this.tcs = tcs;
		}
	}

}
