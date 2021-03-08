
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Animator))]
public class ReadyButtons : MonoBehaviour, IOnGameStart, IOnGameInit {

	[SerializeField] bool allowEnemyReady;
	[SerializeField] ReadyButton buttonPrefab;

	void Awake() => Game.onEvents.Add(this);
	void OnDestroy() { if (Game.game) Game.onEvents.Remove(this); }

	void IOnGameInit.OnGameInit() {
		var teams = Game.game.mode.teams;
		foreach (var team in teams) {
			var readyButton = Instantiate(buttonPrefab, transform);
			readyButton.team = team;
			readyButton.text.text = $"Ready {team}";
			if (team == Game.game.team) readyButton.transform.SetAsFirstSibling();
			if (!allowEnemyReady && team != Game.game.team) {
				readyButton.button.interactable = readyButton.button.enabled = false;
			}
		}
	}

	void IOnGameStart.OnGameStart() {
		foreach (var readyButton in GetComponentsInChildren<ReadyButton>()) {
			readyButton.button.enabled = false;
		}
		GetComponent<Animator>().SetTrigger("Hide");
	}

}