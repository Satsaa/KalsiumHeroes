
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Muc.Components.Extended;

/// <summary> Game handler. Literally the thing that makes the game work. </summary>
[DefaultExecutionOrder(-600)]
[RequireComponent(typeof(GameEvents))]
[RequireComponent(typeof(Rounds))]
[RequireComponent(typeof(Targeting))]
[RequireComponent(typeof(TileGrid))]
public class Game : Singleton<Game> {

	public static Game game => instance;

	public static GameEvents events => instance._events;
	public static Rounds rounds => instance._rounds;
	public static Targeting targeting => instance._targeting;
	public static TileGrid grid => instance._grid;
	public static ObjectDict<DataObject> dataObjects => instance._dataObjects;
	public static Hooks<IGlobalHook> hooks => instance._hooks;

	[SerializeField, HideInInspector] GameEvents _events;
	[SerializeField, HideInInspector] Rounds _rounds;
	[SerializeField, HideInInspector] Targeting _targeting;
	[SerializeField, HideInInspector] TileGrid _grid;
	[SerializeField] ObjectDict<DataObject> _dataObjects = new ObjectDict<DataObject>();
	[SerializeField] Hooks<IGlobalHook> _hooks = new Hooks<IGlobalHook>();

	[field: SerializeField] public bool inited { get; private set; }
	[field: SerializeField] public bool started { get; private set; }
	[field: SerializeField] public int gameEventNum { get; internal set; }
	[field: SerializeField] public List<Team> readied { get; private set; }

	[field: SerializeField] public Team team { get; private set; }
	[field: SerializeField] public string code { get; private set; }
	[field: SerializeField] public GameMode mode { get; private set; }

	protected void Reset() {
		_events = GetComponent<GameEvents>();
		_rounds = GetComponent<Rounds>();
		_targeting = GetComponent<Targeting>();
		_grid = GetComponent<TileGrid>();
	}

	new protected void Awake() {
		base.Awake();
		Flush();
	}

	protected void Start() {
		foreach (var team in mode.teams) {
			for (int i = 0; i < mode.draft.Length; i++) {
				var unitId = mode.draft[i];
				var unitSrc = App.library.GetById<UnitData>(unitId);
				var actor = Instantiate(unitSrc.actor);
				var spawn = actor.gameObject.AddComponent<SpawnControl>();
				spawn.source = unitSrc;
				spawn.team = team;
				if (team == Game.game.team) {
					Tile tile = i >= mode.draftPositions.Length ? null : Game.grid.GetTile(mode.draftPositions[i]);
					if (tile) spawn.SetTile(tile);
				} else {
					Tile tile = i >= mode.altDraftPositions.Length ? null : Game.grid.GetTile(mode.altDraftPositions[i]);
					if (tile) spawn.SetTile(tile);
				}
			}
		}
	}

	void Update() {
		using (var scope = new Hooks.Scope()) _hooks.ForEach<IOnUpdate>(scope, v => v.OnUpdate());
	}

	void LateUpdate() {
		using (var scope = new Hooks.Scope()) _hooks.ForEach<IOnLateUpdate>(scope, v => v.OnLateUpdate());
	}

	/// <summary> Initializes the game </summary>
	public void Init(string code, Team team) {
		if (String.IsNullOrWhiteSpace(code)) throw new ArgumentException($"Code is empty.", nameof(code));
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
		_rounds.OnGameStart();
		using (var scope = new Hooks.Scope()) Game.hooks.ForEach<IOnGameStart>(scope, v => v.OnGameStart());
	}

	/// <summary> Removes removed DataObjects from the cache and destroys them </summary>
	private void Flush() {
		foreach (var dataObject in dataObjects.Get<Master>().Where(v => v.removed).ToList()) {
			dataObjects.Remove(dataObject);
			ObjectUtil.Destroy(dataObject);
		}
	}

}