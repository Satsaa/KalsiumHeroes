
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
  private static Game _instance;

  public static GameClient client => instance._client;
  public static GameGrid grid => instance._grid;

  public static Logic logic => instance._logic;
  public static Anims anims => instance._anims;

  [SerializeField] private GameClient _client = new GameClient();
  [SerializeField] private GameGrid _grid;

  [SerializeField] private Logic _logic = new Logic();
  [SerializeField] private Anims _anims = new Anims();

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

  public bool TryRunNextAnim() => anims.TryRunNextAnim();

}



#if UNITY_EDITOR

[CustomEditor(typeof(Game))]
public class GameEditor : Editor {

  Game t => (Game)target;


  public override void OnInspectorGUI() {

    DrawDefaultInspector();

    if (GUILayout.Button(nameof(t.TryRunNextAnim))) {
      var res = t.TryRunNextAnim();
      Debug.Log($"{nameof(t.TryRunNextAnim)} => {res}");
    }
  }

}
#endif