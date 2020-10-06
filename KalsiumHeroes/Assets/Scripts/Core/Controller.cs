
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Controller : MonoBehaviour {

  private RoundManager rm => Game.rounds;
  private Events e => Game.events;
  private bool finished => e.finished;

  [SerializeField] ControlFlow flow;

  void Start() {

  }

  void Update() {
    if (!finished && flow == null) {

    }
  }

}
