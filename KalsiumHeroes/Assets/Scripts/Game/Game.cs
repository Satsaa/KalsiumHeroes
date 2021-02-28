
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary> Game handler. Literally the thing that makes the game work. </summary>
[DefaultExecutionOrder(-600)]
[RequireComponent(typeof(Events))]
[RequireComponent(typeof(Rounds))]
[RequireComponent(typeof(Targeting))]
[RequireComponent(typeof(TileGrid))]
public class Game : Singleton<Game> {

	public static Events events => instance._events;
	public static Rounds rounds => instance._rounds;
	public static Targeting targeting => instance._targeting;
	public static TileGrid grid => instance._grid;
	public static GameMode mode => instance._mode;
	public static ObjectDict<DataObject> dataObjects => instance._dataObjects;
	public static OnEvents<IGlobalOnEvent> onEvents => instance._onEvents;
	public static bool started => instance._started;

	[SerializeField, HideInInspector] Events _events;
	[SerializeField, HideInInspector] Rounds _rounds;
	[SerializeField, HideInInspector] Targeting _targeting;
	[SerializeField, HideInInspector] TileGrid _grid;
	[SerializeField] GameMode _mode;
	[SerializeField] ObjectDict<DataObject> _dataObjects = new ObjectDict<DataObject>();
	[SerializeField] OnEvents<IGlobalOnEvent> _onEvents = new OnEvents<IGlobalOnEvent>();
	[SerializeField] bool _started;

	public int readyCount;
	public int gameEventNum;
	public Team team;

	protected void Reset() {
		_events = GetComponent<Events>();
		_rounds = GetComponent<Rounds>();
		_targeting = GetComponent<Targeting>();
		_grid = GetComponent<TileGrid>();
	}

	new protected void Awake() {
		base.Awake();
		Flush();
	}

	protected void Start() {
		foreach (var unitId in Game.mode.draft) {
			var unitData = App.library.GetById<UnitData>(unitId);
			var actor = Instantiate(unitData.actor);
			var spawn = actor.gameObject.AddComponent<SpawnControl>();
			spawn.source = unitData;
			spawn.team = Team.Team1;
		}
		// !!! Enemy team
		foreach (var unitId in Game.mode.draft) {
			var unitData = App.library.GetById<UnitData>(unitId);
			var actor = Instantiate(unitData.actor);
			var spawn = actor.gameObject.AddComponent<SpawnControl>();
			spawn.source = unitData;
			spawn.team = Team.Team2;
		}
	}

	void Update() {
		if (_started) {
			_events.Update();
		}
		using (var scope = new OnEvents.Scope()) _onEvents.ForEach<IOnUpdate>(scope, v => v.OnUpdate());
	}

	void LateUpdate() {
		if (_started) {
			using (var scope = new OnEvents.Scope()) _onEvents.ForEach<IOnLateUpdate>(scope, v => v.OnLateUpdate());
		}
	}

	/// <summary> Removes removed DataObjects from the cache and destroys them </summary>
	private void Flush() {
		foreach (var dataObject in dataObjects.Get<Master>().Where(v => v.removed).ToList()) {
			dataObjects.Remove(dataObject);
			ObjectUtil.Destroy(dataObject);
		}
	}

	/// <summary> Removes removed DataObjects from the cache and destroys them </summary>
	public void StartGame() {
		if (_started) {
			Debug.LogWarning("Called StartGame after the Game had already been started");
			return;
		}
		_started = true;
		_rounds.OnGameStart();
		using (var scope = new OnEvents.Scope()) Game.onEvents.ForEach<IOnGameStart>(scope, v => v.OnGameStart());
	}
}