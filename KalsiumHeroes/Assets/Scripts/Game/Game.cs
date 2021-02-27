
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary> Game handler. Literally the thing that makes the game work. </summary>
[DefaultExecutionOrder(-600)]
[RequireComponent(typeof(TileGrid), typeof(Targeting))]
public class Game : Singleton<Game> {

	public static Client client => instance._client;
	public static Events events => instance._events;
	public static GameMode mode => instance._mode;
	public static Rounds rounds => instance._rounds;
	public static Targeting targeting => instance._targeting;
	public static TileGrid grid => instance._grid;
	public static ObjectDict<DataObject> dataObjects => instance._dataObjects;
	public static OnEvents<IGlobalOnEvent> onEvents => instance._onEvents;

	[SerializeField] private Client _client = new Client();
	[SerializeField] private Events _events = new Events();
	[SerializeField] private GameMode _mode;
	[SerializeField] private Rounds _rounds = new Rounds();
	[SerializeField] private Targeting _targeting = default;
	[SerializeField] private TileGrid _grid;
	[SerializeField] private ObjectDict<DataObject> _dataObjects = new ObjectDict<DataObject>();
	[SerializeField] private OnEvents<IGlobalOnEvent> _onEvents = new OnEvents<IGlobalOnEvent>();

	public static int readyCount { get => instance._readyCount; set => instance._readyCount = value; }
	[SerializeField] private int _readyCount = 0;

	protected void OnValidate() => Awake();
	new protected void Awake() {
		base.Awake();
		if (!_grid) Debug.Assert(_grid = GetComponent<TileGrid>());
		if (!_targeting) Debug.Assert(_targeting = GetComponent<Targeting>());
		Flush();
	}

	/// <summary> Removes removed DataObjects from the cache and destroys them </summary>
	private void Flush() {
		foreach (var dataObject in dataObjects.Get<Master>().Where(v => v.removed).ToList()) {
			dataObjects.Remove(dataObject);
			ObjectUtil.Destroy(dataObject);
		}
	}

	private void Start() {
		_rounds.OnGameStart();
		using (var scope = new OnEvents.Scope()) Game.onEvents.ForEach<IOnGameStart>(scope, v => v.OnGameStart());
	}

	void Update() {
		_events.Update();
		using (var scope = new OnEvents.Scope()) _onEvents.ForEach<IOnUpdate>(scope, v => v.OnUpdate());
	}

	void LateUpdate() {
		using (var scope = new OnEvents.Scope()) _onEvents.ForEach<IOnLateUpdate>(scope, v => v.OnLateUpdate());
	}
}