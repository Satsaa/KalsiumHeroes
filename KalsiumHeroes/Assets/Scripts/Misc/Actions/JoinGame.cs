
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class JoinGame : MonoBehaviour {

	[SerializeField] MessageBoxPreset messageBoxPreset;
	[SerializeField] TMP_InputField codeInputPrefab;

	[SerializeField] TextSource title;
	[SerializeField] TextSource message;

	[SerializeField] TextSource join;
	[SerializeField] TextSource cancel;

	public void DoJoinGame() {

		var codeInputField = Instantiate(codeInputPrefab);

		var msgBox = messageBoxPreset.Show(title, message,
			join ?? "JOIN", () => OnJoin(codeInputField.text),
			cancel ?? "CANCEL", null
		);

		msgBox.AddCustomObject(codeInputField.gameObject);

		codeInputField.onValueChanged.AddListener((v) => { OnValueChanged(v, msgBox.options[0]); });
		OnValueChanged("", msgBox.options[0]);
	}

	void OnValueChanged(string code, MessageOption joinOption) {
		joinOption.SetInteractable(!String.IsNullOrWhiteSpace(code));
	}

	async void OnJoin(string code) {
		var task = App.app.JoinGame(code);
		var spinner = App.app.ShowSpinner($"Joining with code {code}", task);
		try {
			var res = await task;
			if (res.errored) {
				App.app.ShowMessage("Error", $"Joining game caused a server error.");
			}
			if (res.failed) {
				App.app.ShowMessage("Fail", $"Joining game failed: {res.message}");
			}
		} catch (System.Exception) {
			App.app.ShowMessage("Error", $"Joining game caused an error.");
			throw;
		}
	}
}
