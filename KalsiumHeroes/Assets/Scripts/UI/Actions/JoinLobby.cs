
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.UI;
using TMPro;

public class JoinLobby : MonoBehaviour {

	public MessageBox messageBox;
	public MessageOption messageOption;
	public GameObject codeInputPrefab;

	public TextSource title;
	public TextSource message;

	public TextSource cancel;
	public TextSource join;

	public void DoJoinLobby() {
		var msgBox = Instantiate(messageBox);
		msgBox.SetTitle(title ?? "TITLE");
		msgBox.SetMessage(message ?? "MESSAGE");
		var codeInputGo = Instantiate(codeInputPrefab);
		var codeInputField = codeInputGo.GetComponentInChildren<TMP_InputField>();
		msgBox.AddCustomObject(codeInputGo);

		// Join
		var joinOption = msgBox.AddOption(messageOption, () => { OnJoin(codeInputField.text); });
		joinOption.SetText(join ?? "JOIN");
		codeInputField.onValueChanged.AddListener((v) => { OnValueChanged(v, joinOption); });
		OnValueChanged(codeInputField.text, joinOption);

		// Cancel
		msgBox.AddOption(messageOption, () => { }).SetText(cancel ?? "CANCEL");
	}

	void OnValueChanged(string code, MessageOption joinOption) {
		joinOption.SetInteractable(!String.IsNullOrWhiteSpace(code));
	}

	void OnJoin(string code) {
		print($"*Joining with code {code}*");
	}
}
