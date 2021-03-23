using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Muc.Data;
using UnityEngine.UI;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using static ClientEvents;

[DefaultExecutionOrder(-900)]
[RequireComponent(typeof(Library))]
[RequireComponent(typeof(Client))]
[RequireComponent(typeof(SharedInput))]
public class App : Singleton<App> {

	public static App app => instance;

	public static Client client => instance._client;
	public static Library library => instance._library;
	public static SharedInput input => instance._input;
	public static Lang lang => instance._lang;
	public static ReadOnlyCollection<GameMode> gameModes => instance._gameModes.AsReadOnly();

	[SerializeField, HideInInspector] Client _client;
	[SerializeField, HideInInspector] Library _library;
	[SerializeField, HideInInspector] SharedInput _input;
	[SerializeField] Lang _lang;
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
		if (!SceneManager.GetSceneByPath(gameScene).isLoaded) {
			Menus.Show(defaultMenu);
		} else {
			Destroy(initDisplay);
		}
	}

	public async Task<Result> CreateGame(string code) {
		return await App.client.Post(new GameCreate() { code = code });
	}

	public async Task<Result> JoinGame(string code) {
		var join = await App.client.Post(new GameJoin() { code = code, team = Team.Team1 });
		switch (join.result) {
			case ResultType.Fail:
				var join2 = await App.client.Post(new GameJoin() { code = code, team = Team.Team2 });
				if (!join2.succeeded) return join2;
				return await JoinGame(code, Team.Team2);
			case ResultType.Success:
				return await JoinGame(code, Team.Team1);
			default:
				return join;
		}
	}

	public async Task<Result> JoinGame(string code, Team team) {
		await Task.Delay(1000);
		var join = await App.client.Post(new GameJoin() { code = code, team = team });
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
