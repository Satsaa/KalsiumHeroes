using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TurnHint : MonoBehaviour {

  [SerializeField, HideInInspector] Text text;

  void Start() {
    text = GetComponent<Text>();
  }

  void Update() {
    if (text && Game.instance) {
      text.text = $"Round: {Game.rounds.round.ToString()}";
    }
  }
}
