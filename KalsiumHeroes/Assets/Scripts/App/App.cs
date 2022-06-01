using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Muc.Data;
using UnityEngine.UI;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using static ClientEvents;
using Muc.Components.Extended;

[DefaultExecutionOrder(-900)]
[RequireComponent(typeof(Library))]
[RequireComponent(typeof(Client))]
[RequireComponent(typeof(SharedInput))]
public class App : Singleton<App> {

	public static App app => instance;

	public static Client client => instance._client;
	public static Library library => instance._library;
	public static SharedInput input => instance._input;
	public static IReadOnlyList<GameMode> gameModes => instance._gameModes;

	[SerializeField, HideInInspector] Client _client;
	[SerializeField, HideInInspector] Library _library;
	[SerializeField, HideInInspector] SharedInput _input;
	[SerializeField] List<GameMode> _gameModes;

	[SerializeField] private InitDisplay initDisplay;
	[SerializeField] private Menu defaultMenu;
	[SerializeField] private SceneReference gameScene;
	[SerializeField] private LoadingSpinner spinner;
	[SerializeField] private Canvas canvas;

	protected void Reset() {
		_client = GetComponent<Client>();
		_library = GetComponent<Library>();
		_input = GetComponent<SharedInput>();
	}

	void Start() {
		// Show the default menu if GameScene not already loaded
		if (SceneManager.GetSceneByPath(gameScene).isLoaded) {
			SceneManager.UnloadSceneAsync(gameScene);
		}
		Menus.Show(defaultMenu);
		Destroy(initDisplay);
	}

	public async Task<Result> CreateGame(string code) {
		return string.IsNullOrWhiteSpace(code)
			? throw new System.ArgumentException($"'{nameof(code)}' cannot be null or whitespace.", nameof(code))
			: await client.Post(new GameCreate() { code = code });
	}

	public async Task<Result> JoinGame(string code, IEnumerable<Team> teams) {
		Result prev = null;
		var sameResults = true;
		foreach (var team in teams) {
			var join = await JoinGame(code, team);
			if (join.succeeded) return join;
			prev ??= join;
			if (prev.result != join.result || prev.message != join.message) {
				sameResults = false;
			}
		}
		return sameResults ? prev : new Result() { result = ResultType.Fail, message = "Server_FailedJoinMultipleReasons" };
	}

	public async Task<Result> JoinGame(string code, Team team) {
		var join = await client.Post(new GameJoin() { code = code, team = team.identifier });
		if (join.succeeded) {
			var res = new TaskCompletionSource<bool>();
			var gameScene = SceneManager.GetSceneByPath(this.gameScene);
			if (gameScene.isLoaded) {
				var unload = SceneManager.UnloadSceneAsync(this.gameScene);
				unload.completed += (op) => {
					var load = SceneManager.LoadSceneAsync(this.gameScene, LoadSceneMode.Additive);
					load.completed += (op) => {
						Game.game.Init(code, team);
						res.SetResult(true);
					};
				};
			} else {
				var load = SceneManager.LoadSceneAsync(this.gameScene, LoadSceneMode.Additive);
				load.completed += (op) => {
					Game.game.Init(code, team);
					res.SetResult(true);
				};
			}
			await res.Task;
			return join;
		}
		return join;
	}

	public async Task<Result> RejoinGame(string code, Team team, int delay = 0) {
		await Task.Delay(delay);
		return await client.Post(new GameJoin() { code = code, team = team.identifier });
	}

	public async Task<Result> LeaveGame(string code) {
		var leave = await client.Post(new GameLeave() { code = code });
		if (leave.succeeded) {
			var res = new TaskCompletionSource<bool>();
			var gameScene = SceneManager.GetSceneByPath(this.gameScene);
			if (gameScene.isLoaded) {
				var unload = SceneManager.UnloadSceneAsync(this.gameScene);
				unload.completed += (op) => {
					res.SetResult(true);
				};
				await res.Task;
			}
			return leave;
		}
		return leave;
	}

	public async Task<Result> DeleteGame(string code) {
		return await client.Post(new GameDelete() { code = code });
	}

	/// <summary> Shows the loading spinner </summary>
	public LoadingSpinner ShowSpinner(string message, Task task) {
		var res = Instantiate(spinner, canvas.transform);
		res.text = message;
		res.SetTask(task);
		return res;
	}

	/// <summary> Shows the loading spinner </summary>
	public LoadingSpinner ShowSpinner(Task task) {
		var res = Instantiate(spinner, canvas.transform);
		res.SetTask(task);
		return res;
	}

	/// <summary> Shows the loading spinner </summary>
	public LoadingSpinner ShowSpinner(string message = null) {
		var res = Instantiate(spinner, canvas.transform);
		res.text = message;
		return res;
	}

}
