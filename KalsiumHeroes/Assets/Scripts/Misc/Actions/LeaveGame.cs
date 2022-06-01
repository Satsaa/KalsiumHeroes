
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class LeaveGame : MonoBehaviour {

	public void DoLeaveGame() {
		OnLeave(Game.instance.code);
	}

	async void OnLeave(string code) {
		var task = App.app.LeaveGame(code);
		var spinner = App.app.ShowSpinner($"Leaving game of code {code}", task);
		try {
			var res = await task;
			if (res.errored) {
				Popups.ShowPopup("Error", "Leaving game caused a server error.");
			}
			if (res.failed) {
				Popups.ShowPopup("Fail", $"Leaving game failed: {res.message}");
			}
		} catch (Exception) {
			Popups.ShowPopup("Error", "Leaving game caused an error.");
			throw;
		}
	}
}
