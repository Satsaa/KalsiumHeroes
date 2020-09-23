

using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using Muc.Extensions;


public class GameTester : MonoBehaviour {

  public Game game;

  void OnValidate() {
    game = GetComponent<Game>();
  }


  void Start() {

  }

  void Update() {

  }
}


#if UNITY_EDITOR

[CustomEditor(typeof(GameTester))]
public class GameTesterEditor : Editor {

  GameTester t => (GameTester)target;


  public override void OnInspectorGUI() {

    DrawDefaultInspector();


    if (GUILayout.Button("Post Event")) {
      var e = new GameEvents.Position();
      t.game.client.PostEvent(e);
    }

  }

}
#endif