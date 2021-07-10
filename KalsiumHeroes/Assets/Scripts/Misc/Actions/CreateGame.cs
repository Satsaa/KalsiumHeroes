
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.UI;
using TMPro;

public class CreateGame : MonoBehaviour {

	[SerializeField] PopupPreset popupPreset;
	[SerializeField] TMP_InputField codeInputPrefab;

	public void DoCreateGame() {

		var codeInputField = Instantiate(codeInputPrefab);

		var msgBox = popupPreset.Show(Lang.GetStr("Popup_CreateGame_Title"), Lang.GetStr("Popup_CreateGame_Message"),
			new PopupPreset.Option(Lang.GetStr("Create"), () => OnCreate(codeInputField.text), PopupOption.Flags.Default),
			new PopupPreset.Option(Lang.GetStr("Cancel"), null, PopupOption.Flags.Cancel)
		);

		msgBox.AddCustomObject(codeInputField.gameObject);

		codeInputField.onValueChanged.AddListener((v) => { OnValueChanged(v, msgBox.options[0]); });
		OnValueChanged("", msgBox.options[0]);
	}

	void OnValueChanged(string code, PopupOption createOption) {
		createOption.SetInteractable(!String.IsNullOrWhiteSpace(code));
	}

	async void OnCreate(string code) {
		var task = App.client.Post(new ClientEvents.GameCreate() {
			code = code,
		});
		try {
			var res = await task;
			if (res.errored) {
				Popups.ShowPopup("Error", $"Game creation caused a server error.");
			}
			if (res.failed) {
				Popups.ShowPopup("Fail", $"Game creation failed: {res.message}");
			}
		} catch (System.Exception) {
			Popups.ShowPopup("Error", $"Game creation caused an error.");
			throw;
		}
	}
}
