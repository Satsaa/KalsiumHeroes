
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.UI;
using TMPro;

public class ReadyTeam : MonoBehaviour {

	[SerializeField] Team team;

	public void DoReadyTeam() {
		var spawns = new List<GameEvents.Ready.SpawnInfo>();
		var spawnCtrls = FindObjectsOfType<SpawnControl>();
		foreach (var spawnCtrl in spawnCtrls) {
			if (spawnCtrl.team == team) {
				spawns.Add(new GameEvents.Ready.SpawnInfo() {
					unit = spawnCtrl.source.identifier,
					position = spawnCtrl.tile.hex.pos,
				});
				Destroy(spawnCtrl.gameObject);
			}
		}
		if (team == Game.instance.team) {
			Game.mode.draftPositions = spawns.Select(v => v.position).ToList();
		} else {
			Game.mode.draftPositionsAlt = spawns.Select(v => v.position).ToList();
		}
		App.client.Post(
			new GameEvents.Ready() {
				spawns = spawns.ToArray(),
				team = team,
			}
		);
	}

}