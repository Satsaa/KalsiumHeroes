
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Muc.Components.Extended;
using System.Threading.Tasks;
using Muc.Editor;
using Serialization;
using Object = UnityEngine.Object;
using UnityEngine.AddressableAssets;

/// <summary> Game handler. Literally the thing that makes the game work. </summary>
[KeepRefToken]
[DefaultExecutionOrder(-600)]
public class GameMaster : Master<UnitModifier, UnitModifierData, IGameHook> {

	/// <summary> Currently active Game instance </summary>
	public static GameMaster game { get; private set; }
	public bool active => game == this;

	[field: SerializeField, ShowEditor] public GameEvents events { get; private set; }
	[field: SerializeField, ShowEditor] public Rounds rounds { get; private set; }
	[field: SerializeField, ShowEditor] public Targeting targeting { get; private set; }
	[field: SerializeField, ShowEditor] public TileGrid grid { get; private set; }
	[field: SerializeField] public ObjectDict<DataObject> dataObjects { get; private set; } = new();

	[field: SerializeField] public bool inited { get; private set; }
	[field: SerializeField] public bool started { get; private set; }
	[field: SerializeField] public List<Team> readied { get; private set; }

	[field: SerializeField] public Team team { get; private set; }
	[field: SerializeField] public string code { get; private set; }
	[field: SerializeField] public GameMode mode { get; private set; }

	[SerializeField] private LoadingSpinner spinner;
	[SerializeField] private RectTransform spinnerParent;

	protected void Reset() {
		events = CreateInstance<GameEvents>();
		rounds = CreateInstance<Rounds>();
		targeting = CreateInstance<Targeting>();
		grid = CreateInstance<TileGrid>();
	}

	protected override void OnCreate() {
		base.OnCreate();
		// sd
	}

	public void Activate() {
		hooks.TryHook(events);
		hooks.TryHook(rounds);
		hooks.TryHook(targeting);
		hooks.TryHook(grid);
		hooks.TryHook(this);
	}

	public void Deactivate() {
		hooks.Unhook(events);
		hooks.Unhook(rounds);
		hooks.Unhook(targeting);
		hooks.Unhook(grid);
		hooks.Unhook(this);
	}

	protected void Start() {
		foreach (var team in mode.teams) {
			for (int i = 0; i < mode.draft.Length; i++) {
				var unitId = mode.draft[i];
				var unitSrc = App.library.GetById<UnitData>(unitId);
				var actor = Instantiate(unitSrc.actor.value);
				var spawn = actor.gameObject.AddComponent<SpawnControl>();
				spawn.source = unitSrc;
				spawn.team = team;
				if (team == Game.game.team) {
					var tile = i >= mode.draftPositions.Length ? null : Game.grid.GetTile(mode.draftPositions[i]);
					if (tile) spawn.SetTile(tile);
				} else {
					var tile = i >= mode.altDraftPositions.Length ? null : Game.grid.GetTile(mode.altDraftPositions[i]);
					if (tile) spawn.SetTile(tile);
				}
			}
		}
	}

	void Update() {
		using (var scope = new Hooks.Scope()) hooks.ForEach<IOnUpdate>(scope, v => v.OnUpdate());
	}

	void LateUpdate() {
		using (var scope = new Hooks.Scope()) hooks.ForEach<IOnLateUpdate>(scope, v => v.OnLateUpdate());
	}

	/// <summary> Initializes the game </summary>
	public void Init(string code, Team team) {
		if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Code is empty.", nameof(code));
		if (!Enum.IsDefined(typeof(Team), team)) throw new ArgumentException($"Invalid team: '{team}'", nameof(team));
		if (!mode.teams.Contains(team)) throw new ArgumentException($"Team not supported by mode. Team: '{team}', Mode: '{Lang.GetStr($"{mode.identifier}_DisplayName")}'", nameof(team));
		if (inited) throw new InvalidOperationException($"Duplicate call to {nameof(Init)}.");
		inited = true;
		this.code = code;
		this.team = team;
		using (var scope = new Hooks.Scope()) Game.hooks.ForEach<IOnGameInit>(scope, v => v.OnGameInit());
	}

	/// <summary> Starts the game </summary>
	public void StartGame() {
		if (started) throw new InvalidOperationException($"Duplicate call to {nameof(StartGame)}.");
		if (!inited) throw new InvalidOperationException($"{nameof(Init)} must be called before calling {nameof(StartGame)}");
		mode.Save();
		started = true;
		rounds.OnGameStart();
		using (var scope = new Hooks.Scope()) Game.hooks.ForEach<IOnGameStart>(scope, v => v.OnGameStart());
	}

	/// <summary> Removes removed DataObjects from the cache and destroys them </summary>
	private void Flush() {
		foreach (var dataObject in dataObjects.Get<Master>().Where(v => v.removed).ToList()) {
			dataObjects.Remove(dataObject);
			ObjectUtil.Destroy(dataObject);
		}
	}

	/// <summary> Shows the loading spinner </summary>
	public LoadingSpinner ShowSpinner(string message, Task task) {
		var res = Instantiate(spinner, spinnerParent);
		res.text = message;
		res.SetTask(task);
		return res;
	}

	/// <summary> Shows the loading spinner </summary>
	public LoadingSpinner ShowSpinner(Task task) {
		var res = Instantiate(spinner, spinnerParent);
		res.SetTask(task);
		return res;
	}

	/// <summary> Shows the loading spinner </summary>
	public LoadingSpinner ShowSpinner(string message = null) {
		var res = Instantiate(spinner, spinnerParent);
		res.text = message;
		return res;
	}

#if UNITY_EDITOR
	[UnityEditor.Callbacks.DidReloadScripts]
	static void OnReloadScripts() {
		// Rejoin old game
		if (Game.game && Application.isPlaying && !String.IsNullOrEmpty(Game.game.code)) {
			App.app.ShowSpinner("Rejoining game (script reload)", App.app.RejoinGame(Game.game.code, Game.game.team, 2000));
		}
	}

#endif
}

#if UNITY_EDITOR
namespace Editors {

	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using Object = UnityEngine.Object;
	using static Muc.Editor.PropertyUtil;
	using static Muc.Editor.EditorUtil;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(GameMaster), true)]
	public class GameMasterEditor : Editor {

		GameMaster t => (GameMaster)target;

		public override void OnInspectorGUI() {
			DrawDefaultInspector();
			if (ButtonField(new("Show all"))) {
				foreach (var dataObject in Game.dataObjects.Get()) {
					dataObject.Show();
				}
			}
			if (ButtonField(new("Hide all"))) {
				foreach (var dataObject in Game.dataObjects.Get()) {
					dataObject.Hide();
				}
			}
		}
	}
}
#endif