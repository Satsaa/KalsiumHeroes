
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[RequireComponent(typeof(Game))]
public class GameAutoStarter : MonoBehaviour {

	[SerializeField] string code = "TEST";
	[SerializeField] Team team = Team.Team1;

	void Start() {
		Game.game.Init(code, team);
	}

}