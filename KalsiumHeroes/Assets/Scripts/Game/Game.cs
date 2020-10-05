
using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary> Game handler. Literally the thing that makes the game work. </summary>
[RequireComponent(typeof(GameGrid))]
public class Game : MonoBehaviour {

  public static Game instance => _instance;
  public static Client client => instance._client;
  public static GameGrid grid => instance._grid;
  public static Events events => instance._events;

  private static Game _instance;
  [SerializeField] private Client _client = new Client();
  [SerializeField] private GameGrid _grid;
  [SerializeField] private Events _events = new Events();

  private void OnValidate() => Awake();
  private void Awake() {

    if (_instance != null && _instance != this) {
      Debug.LogError($"Multiple {nameof(Game)} managers. Exterminating.");
      Destroy(this);
      return;
    }

    _instance = this;
    if (_grid == null) _grid = GetComponent<GameGrid>();
  }

  void Update() {
    // TryRunNextAnim();
  }

  public bool TryNextEvent() => events.NextEvent();

}



#if UNITY_EDITOR

[CustomEditor(typeof(Game))]
public class GameEditor : Editor {

  Game t => (Game)target;


  public override void OnInspectorGUI() {

    DrawDefaultInspector();

    if (GUILayout.Button(nameof(t.TryNextEvent))) {
      var res = t.TryNextEvent();
      Debug.Log($"{nameof(t.TryNextEvent)} => {res}");
    }
  }

}
#endif