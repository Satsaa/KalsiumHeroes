using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameModeDropDown : TMP_Dropdown {

	new protected void Start() {
		base.Start();
		options.Clear();
		foreach (var gameMode in App.gameModes) {
			options.Add(new OptionData($"{Lang.GetStr($"{gameMode}_DisplayName")} {gameMode.version}"));
		}
		value = 0;
	}
}

