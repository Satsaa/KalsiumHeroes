
using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary> Game handler. Literally the thing that makes the game work. </summary>
[RequireComponent(typeof(TileGrid), typeof(Targeting))]
public class Game : MonoBehaviour {

	public static Game instance => _instance;
	public static TileGrid grid => instance._grid;
	public static Client client => instance._client;
	public static Events events => instance._events;
	public static Targeting targeting => instance._targeting;
	public static RoundManager rounds => instance._rounds;

	public static DataComponentDict dataComponents => instance._dataComponents;

	private static Game _instance;
	[SerializeField] private TileGrid _grid;
	[SerializeField] private Client _client = new Client();
	[SerializeField] private Events _events = new Events();
	[SerializeField] private Targeting _targeting = default;
	[SerializeField] private RoundManager _rounds = new RoundManager();
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
		dataComponents.BuildFromScene();
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
		dataComponents.BuildFromScene();
		_rounds.OnGameStart();
	}

	void Update() {
		_events.Update();
	}
}