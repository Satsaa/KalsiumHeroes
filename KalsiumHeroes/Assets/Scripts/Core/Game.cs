
using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary> Game handler. Literally the thing that makes the game work. </summary>
[RequireComponent(typeof(GameGrid), typeof(Targeting))]
public class Game : MonoBehaviour {

  public static Game instance => _instance;
  public static GameGrid grid => instance._grid;
  public static Client client => instance._client;
  public static Events events => instance._events;
  public static Targeting targeting => instance._targeting;
  public static RoundManager rounds => instance._rounds;
  public static ModifierTracker modifiers => instance._modifiers;

  private static Game _instance;
  [SerializeField] private GameGrid _grid;
  [SerializeField] private Client _client = new Client();
  [SerializeField] private Events _events = new Events();
  [SerializeField] private Targeting _targeting = default;
  [SerializeField] private RoundManager _rounds = new RoundManager();
  [SerializeField] private ModifierTracker _modifiers = new ModifierTracker();

  private void OnValidate() => Awake();
  private void Awake() {

    if (_instance != null && _instance != this) {
      Debug.LogError($"Multiple {nameof(Game)} managers. Exterminating.");
      Destroy(this);
      return;
    }

    _instance = this;
    if (_grid == null) _grid = GetComponent<GameGrid>();
    modifiers.ResetModifiers();
    _rounds.OnGameStart();
  }

  void Update() {
    _events.NextEvent();
  }
}