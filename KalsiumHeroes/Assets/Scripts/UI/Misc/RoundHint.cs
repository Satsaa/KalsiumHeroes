﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class RoundHint : MonoBehaviour {

	[SerializeField, HideInInspector] Text text;

	void Start() {
		text = GetComponent<Text>();
	}

	void Update() {
		if (text && Game.game) {
			text.text = $"Round: {Game.rounds.round}";
		}
	}
}
