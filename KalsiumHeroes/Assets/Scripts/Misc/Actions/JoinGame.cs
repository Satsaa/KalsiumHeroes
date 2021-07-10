
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class JoinGame : MonoBehaviour {

	[SerializeField] PopupPreset popupPreset;
	[SerializeField] TMP_InputField codeInputPrefab;

	public void DoJoinGame() {

		var codeInputField = Instantiate(codeInputPrefab);

		var msgBox = popupPreset.Show(Lang.GetStr("Popup_JoinGame_Title"), Lang.GetStr("Popup_JoinGame_Message"),
			new PopupPreset.Option(Lang.GetStr("Join"), () => OnJoin(codeInputField.text), PopupOption.Flags.Default),
			new PopupPreset.Option(Lang.GetStr("Cancel"), null, PopupOption.Flags.Cancel)
		);

		msgBox.AddCustomObject(codeInputField.gameObject);

		codeInputField.onValueChanged.AddListener((v) => { OnValueChanged(v, msgBox.options[0]); });
		OnValueChanged("", msgBox.options[0]);
	}

	void OnValueChanged(string code, PopupOption joinOption) {
		joinOption.SetInteractable(!String.IsNullOrWhiteSpace(code));
	}

	async void OnJoin(string code) {
		var task = App.app.JoinGame(code);
		var spinner = App.app.ShowSpinner($"Joining with code {code}", task);
		try {
			var res = await task;
			if (res.errored) {
				Popups.ShowPopup("Error", $"Joining game caused a server error.");
			}
			if (res.failed) {
				Popups.ShowPopup("Fail", $"Joining game failed: {res.message}");
			}
		} catch (System.Exception) {
			Popups.ShowPopup("Error", $"Joining game caused an error.");
			throw;
		}
	}
}
