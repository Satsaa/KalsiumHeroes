
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.UI;
using TMPro;

public class CreateGame : MonoBehaviour {

	[SerializeField] MessageBoxPreset messageBoxPreset;
	[SerializeField] TMP_InputField codeInputPrefab;

	[SerializeField] TextSource title;
	[SerializeField] TextSource message;

	[SerializeField] TextSource create;
	[SerializeField] TextSource cancel;

	public void DoCreateGame() {

		var codeInputField = Instantiate(codeInputPrefab);

		var msgBox = messageBoxPreset.Show(title, message,
			create ?? "CREATE", () => OnCreate(codeInputField.text),
			cancel ?? "CANCEL", null
		);

		msgBox.AddCustomObject(codeInputField.gameObject);

		codeInputField.onValueChanged.AddListener((v) => { OnValueChanged(v, msgBox.options[0]); });
		OnValueChanged("", msgBox.options[0]);
	}

	void OnValueChanged(string code, MessageOption createOption) {
		createOption.SetInteractable(!String.IsNullOrWhiteSpace(code));
	}

	async void OnCreate(string code) {
		var task = App.client.Post(new ClientEvents.GameCreate() {
			code = code,
		});
		try {
			var res = await task;
			if (res.errored) {
				App.app.ShowMessage("Error", $"Game creation caused a server error.");
			}
			if (res.failed) {
				App.app.ShowMessage("Fail", $"Game creation failed: {res.message}");
			}
		} catch (System.Exception) {
			App.app.ShowMessage("Error", $"Game creation caused an error.");
			throw;
		}
	}
}
