
using System.Collections.Generic;
using UnityEngine;
using Grids;

/// <summary> Game handler. Literally the thing that makes the game work. </summary>
[ExecuteAlways]
public class Game : MonoBehaviour {

  public static Game Instance { get { return _instance; } }
  private static Game _instance;

  [SerializeField] public GameClient client;

  [SerializeField] public LogicAPI logic;
  [SerializeField] public AnimationAPI anim;

  [SerializeReference] public List<object> _animationStack = new List<object>();
  [SerializeField] public int currentAnimationIndex;
  [SerializeField] public GameEvent currentAnimation => (GameEvent)_animationStack[currentAnimationIndex];


  private void Reset() => Awake();
  private void Awake() {

    if (_instance != null && _instance != this) {
      Debug.LogError($"Multiple {nameof(Game)} managers.");
      DestroyImmediate(this.gameObject);
      return;
    }

    if (client == null) client = GameClient.CreateInstance<GameClient>();
    if (logic == null) logic = LogicAPI.CreateInstance<LogicAPI>();
    if (anim == null) anim = AnimationAPI.CreateInstance<AnimationAPI>();

    _instance = this;
  }

}