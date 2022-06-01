
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class DeleteGame : MonoBehaviour {

	[SerializeField] PopupPreset popupPreset;
	[SerializeField] TMP_InputField codeInputPrefab;

	public void DoDeleteGame() {

		var codeInputField = Instantiate(codeInputPrefab);

		var msgBox = popupPreset.Show(Lang.GetStr("Popup_DeleteGame_Title"), Lang.GetStr("Popup_DeleteGame_Message"),
			new PopupPreset.Option(Lang.GetStr("Delete"), () => OnDelete(codeInputField.text), PopupOption.Flags.Default),
			new PopupPreset.Option(Lang.GetStr("Cancel"), null, PopupOption.Flags.Cancel)
		);

		msgBox.AddCustomObject(codeInputField.gameObject);

		codeInputField.onValueChanged.AddListener((v) => OnValueChanged(v, msgBox.options[0]));
		OnValueChanged("", msgBox.options[0]);
	}

	void OnValueChanged(string code, PopupOption deleteOption) {
		deleteOption.SetInteractable(!String.IsNullOrWhiteSpace(code));
	}

	async void OnDelete(string code) {
		var task = App.app.DeleteGame(code);
		var spinner = App.app.ShowSpinner($"Deleting game of code {code}", task);
		try {
			var res = await task;
			if (res.errored) {
				Popups.ShowPopup("Error", "Deleting game caused a server error.");
			}
			if (res.failed) {
				Popups.ShowPopup("Fail", $"Deleting game failed: {res.message}");
			}
		} catch (Exception) {
			Popups.ShowPopup("Error", "Deleting game caused an error.");
			throw;
		}
	}
}
