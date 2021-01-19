
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary> Game handler. Literally the thing that makes the game work. </summary>
[DefaultExecutionOrder(-600)]
[RequireComponent(typeof(TileGrid), typeof(Rounds), typeof(Targeting))]
public class Game : MonoBehaviour {

	public static Game instance => _instance;
	public static TileGrid grid => instance._grid;
	public static Client client => instance._client;
	public static Events events => instance._events;
	public static Targeting targeting => instance._targeting;
	public static Rounds rounds => instance._rounds;
	public static Library library => instance._library;
	public static DataComponentDict dataComponents => instance._dataComponents;
	public static OnEvents<IGlobalOnEvent> onEvents => instance._onEvents;

	private static Game _instance;
	[SerializeField] private TileGrid _grid;
	[SerializeField] private Client _client = new Client();
	[SerializeField] private Events _events = new Events();
	[SerializeField] private Targeting _targeting = default;
	[SerializeField] private Rounds _rounds = new Rounds();
	[SerializeField] private Library _library;
	[SerializeField] private DataComponentDict _dataComponents = new DataComponentDict();
	[SerializeField] private OnEvents<IGlobalOnEvent> _onEvents = new OnEvents<IGlobalOnEvent>();

	public static int readyCount { get => instance._readyCount; set => instance._readyCount = value; }
	[SerializeField] private int _readyCount = 0;

	private void OnValidate() => Awake();

	private void Awake() {

		if (_instance != null && _instance != this) {
			Debug.LogError($"Multiple {nameof(Game)} GameObjects. Exterminating.");
			ObjectUtil.Destroy(this);
			return;
		}

		_instance = this;
		if (_grid == null && (_grid = GetComponent<TileGrid>()) == null) Debug.LogError($"No {nameof(TileGrid)} Component!");
		if (_targeting == null && (_targeting = GetComponent<Targeting>()) == null) Debug.LogError($"No {nameof(Targeting)} Component!");
	}

	private void Start() {
		_rounds.OnGameStart();
		using (var scope = new OnEvents.Scope()) Game.onEvents.ForEach<IOnGameStart>(scope, v => v.OnGameStart());
	}

	void Update() {
		_events.Update();
	}
}