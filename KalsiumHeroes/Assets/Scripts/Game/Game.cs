
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
	public static OnEvents<IGlobalOnEvent> onEvents => instance._onEvents;

	[SerializeField, HideInInspector] GameEvents _events;
	[SerializeField, HideInInspector] Rounds _rounds;
	[SerializeField, HideInInspector] Targeting _targeting;
	[SerializeField, HideInInspector] TileGrid _grid;
	[SerializeField] ObjectDict<DataObject> _dataObjects = new ObjectDict<DataObject>();
	[SerializeField] OnEvents<IGlobalOnEvent> _onEvents = new OnEvents<IGlobalOnEvent>();

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
				var unitData = App.library.GetById<UnitData>(unitId);
				var actor = Instantiate(unitData.actor);
				var spawn = actor.gameObject.AddComponent<SpawnControl>();
				spawn.source = unitData;
				spawn.team = team;
				if (mode.draftPositions.TryGetValue(team, out var positions)) {
					Tile tile = i >= positions.Count ? null : Game.grid.GetTile(positions[i]);
					if (tile) spawn.SetTile(tile);
				}
			}
		}
	}

	void Update() {
		if (!started) return;
		_events.Update();
		using (var scope = new OnEvents.Scope()) _onEvents.ForEach<IOnUpdate>(scope, v => v.OnUpdate());
	}

	void LateUpdate() {
		if (!started) return;
		using (var scope = new OnEvents.Scope()) _onEvents.ForEach<IOnLateUpdate>(scope, v => v.OnLateUpdate());
	}

	/// <summary> Initializes the game </summary>
	public void Init(string code, Team team) {
		if (String.IsNullOrWhiteSpace(code)) throw new ArgumentException($"Code is empty.", nameof(code));
		if (!Enum.IsDefined(typeof(Team), team)) throw new ArgumentException($"Invalid team: '{team}'", nameof(team));
		if (!mode.teams.Contains(team)) throw new ArgumentException($"Team not supported by mode. Team: '{team}', Mode: '{mode.title}'", nameof(team));
		if (inited) throw new InvalidOperationException($"Duplicate call to {nameof(Init)}.");
		inited = true;
		using (var scope = new OnEvents.Scope()) Game.onEvents.ForEach<IOnGameInit>(scope, v => v.OnGameInit());
	}

	/// <summary> Starts the game </summary>
	public void StartGame() {
		if (started) throw new InvalidOperationException($"Duplicate call to {nameof(StartGame)}.");
		if (!inited) throw new InvalidOperationException($"{nameof(Init)} must be called before calling {nameof(StartGame)}");
		started = true;
		_rounds.OnGameStart();
		using (var scope = new OnEvents.Scope()) Game.onEvents.ForEach<IOnGameStart>(scope, v => v.OnGameStart());
	}

	/// <summary> Removes removed DataObjects from the cache and destroys them </summary>
	private void Flush() {
		foreach (var dataObject in dataObjects.Get<Master>().Where(v => v.removed).ToList()) {
			dataObjects.Remove(dataObject);
			ObjectUtil.Destroy(dataObject);
		}
	}

}