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
public class App : Singleton<App> {

	public static App app => instance;

	public static Client client => instance._client;
	public static Library library => instance._library;
	public static Lang lang => instance._lang;
	public static ReadOnlyCollection<GameMode> gameModes => instance._gameModes.AsReadOnly();

	[SerializeField, HideInInspector] Client _client;
	[SerializeField, HideInInspector] Library _library;
	[SerializeField] Lang _lang;
	[SerializeField] List<GameMode> _gameModes;

	[SerializeField] private InitDisplay initDisplay;
	[SerializeField] private SceneReference defaultScene;
	[SerializeField] private SceneReference gameScene;
	[SerializeField] private LoadingSpinner spinner;
	[SerializeField] private MessageBoxPreset defaultMessageBox;
	[SerializeField] private Canvas canvas;

	protected void Reset() {
		_library = GetComponent<Library>();
		_client = GetComponent<Client>();
	}

	void Start() {
		// Load default scene if only the App scene is loaded
		if (SceneManager.sceneCount == 1) {
			initDisplay.op = SceneManager.LoadSceneAsync(defaultScene.scenePath, LoadSceneMode.Additive);
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
		await Task.Delay(2000);
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

	/// <summary> Shows the default messagebox </summary>
	public MessageBox ShowMessage(string title, string message) {
		var res = defaultMessageBox.Show(title, message);
		return res;
	}
}
