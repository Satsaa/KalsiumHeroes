
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class ReadyButton : MonoBehaviour, IOnTeamReady {

	public Team team;
	public Button button;
	public TMP_Text text;
	[SerializeField] Color readyColor;

	void Awake() {
		button = GetComponent<Button>();
		text = GetComponentInChildren<TMP_Text>();
		Game.onEvents.Add(this);
	}

	void OnDestroy() {
		if (Game.game) Game.onEvents.Remove(this);
	}

	public void DoReadyTeam() {
		var spawns = new List<GameEvents.Ready.SpawnInfo>();
		var spawnCtrls = FindObjectsOfType<SpawnControl>();
		foreach (var spawnCtrl in spawnCtrls) {
			if (spawnCtrl.team == team) {
				spawns.Add(new GameEvents.Ready.SpawnInfo() {
					unit = spawnCtrl.source.identifier,
					position = spawnCtrl.tile.hex.pos,
				});
			}
		}
		if (team == Game.game.team) {
			Game.game.mode.draftPositions = spawns.Select(v => v.position).ToArray();
		} else {
			Game.game.mode.altDraftPositions = spawns.Select(v => v.position).ToArray();
		}
		App.client.Post(
			new GameEvents.Ready() {
				spawns = spawns.ToArray(),
				team = team,
			}
		);
	}

	void IOnTeamReady.OnTeamReady(Team team) {
		if (team != this.team) return;
		button.targetGraphic.color = readyColor;
	}

}