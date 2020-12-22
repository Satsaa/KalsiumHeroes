
using System;
using System.Collections.Generic;
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

	public static DataComponentDict dataComponents => instance._dataComponents;

	private static Game _instance;
	[SerializeField] private TileGrid _grid;
	[SerializeField] private Client _client = new Client();
	[SerializeField] private Events _events = new Events();
	[SerializeField] private Targeting _targeting = default;
	[SerializeField] private Rounds _rounds = new Rounds();
	[SerializeField] private DataComponentDict _dataComponents = new DataComponentDict();

	private void OnValidate() {

		if (_instance != null && _instance != this) {
			Debug.LogError($"Multiple {nameof(Game)} managers. Exterminating.");
			DestroyImmediate(this);
			return;
		}

		_instance = this;
		if (_grid == null && (_grid = GetComponent<TileGrid>()) == null) Debug.LogError($"No {nameof(TileGrid)} Component!");
		if (_targeting == null && (_targeting = GetComponent<Targeting>()) == null) Debug.LogError($"No {nameof(Targeting)} Component!");
	}

	private void Awake() {

		if (_instance != null && _instance != this) {
			Debug.LogError($"Multiple {nameof(Game)} managers. Exterminating.");
			Destroy(this);
			return;
		}

		_instance = this;
		if (_grid == null) _grid = GetComponent<TileGrid>();
		if (_targeting == null) _targeting = GetComponent<Targeting>();
		_rounds.OnGameStart();
		foreach (var modifier in dataComponents.Get<Modifier>()) {
			modifier.OnGameStart();
		}
	}

	void Update() {
		_events.Update();
	}
}