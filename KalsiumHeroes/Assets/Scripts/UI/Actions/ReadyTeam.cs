
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
		var spawns = new List<Events.Ready.SpawnInfo>();
		var spawnCtrls = FindObjectsOfType<SpawnControl>();
		foreach (var spawnCtrl in spawnCtrls) {
			if (spawnCtrl.team == team) {
				spawns.Add(new Events.Ready.SpawnInfo() {
					unit = spawnCtrl.source.identifier,
					team = team,
					position = spawnCtrl.tile.hex.pos,
				});
				Destroy(spawnCtrl.gameObject);
			}
		}
		App.client.PostEvent(
			new Events.Ready() {
				spawns = spawns.ToArray(),
			}
		);
	}

}
