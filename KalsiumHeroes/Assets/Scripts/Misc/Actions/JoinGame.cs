
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.UI;
using TMPro;

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

	void OnJoin(string code) {
		print($"*Joining with code {code}*");
	}
}
