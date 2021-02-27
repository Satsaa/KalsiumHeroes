using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Muc.Data;
using UnityEngine.UI;
using System;
using TMPro;
using Object = UnityEngine.Object;

[CreateAssetMenu(fileName = nameof(MessageBoxPreset), menuName = "KalsiumHeroes/" + nameof(MessageBoxPreset))]
public class MessageBoxPreset : ScriptableObject {

	[SerializeField] MessageBox messageBoxPrefab;
	[SerializeField] MessageOption messageOptionPrefab;

	[SerializeField] TextSource baseTitle;
	[SerializeField] TextSource baseMessage;
	[SerializeField] TextSource baseOptionText;


	public MessageBox Show(string title = null, string message = null
	) {
		var msgBox = Instantiate(messageBoxPrefab);
		msgBox.SetTitle(title ?? baseTitle);
		msgBox.SetMessage(message ?? baseMessage);
		msgBox.AddOption(messageOptionPrefab).SetText(baseOptionText);
		return msgBox;
	}

	public MessageBox Show(string title, string message,
		string optionText1, Action optionAction1
	) {
		var msgBox = Instantiate(messageBoxPrefab);
		msgBox.SetTitle(title);
		msgBox.SetMessage(message);
		msgBox.AddOption(messageOptionPrefab, optionAction1).SetText(optionText1);
		return msgBox;
	}

	public MessageBox Show(string title, string message,
		string optionText1, Action optionAction1,
		string optionText2, Action optionAction2
	) {
		var msgBox = Instantiate(messageBoxPrefab);
		msgBox.SetTitle(title);
		msgBox.SetMessage(message);
		msgBox.AddOption(messageOptionPrefab, optionAction1).SetText(optionText1);
		msgBox.AddOption(messageOptionPrefab, optionAction2).SetText(optionText2);
		return msgBox;
	}

	public MessageBox Show(string title, string message,
		string optionText1, Action optionAction1,
		string optionText2, Action optionAction2,
		string optionText3, Action optionAction3
	) {
		var msgBox = Instantiate(messageBoxPrefab);
		msgBox.SetTitle(title);
		msgBox.SetMessage(message);
		msgBox.AddOption(messageOptionPrefab, optionAction1).SetText(optionText1);
		msgBox.AddOption(messageOptionPrefab, optionAction2).SetText(optionText2);
		msgBox.AddOption(messageOptionPrefab, optionAction3).SetText(optionText3);
		return msgBox;
	}

	public MessageBox Show(string title, string message,
		string optionText1, Action optionAction1,
		string optionText2, Action optionAction2,
		string optionText3, Action optionAction3,
		string optionText4, Action optionAction4
	) {
		var msgBox = Instantiate(messageBoxPrefab);
		msgBox.SetTitle(title);
		msgBox.SetMessage(message);
		msgBox.AddOption(messageOptionPrefab, optionAction1).SetText(optionText1);
		msgBox.AddOption(messageOptionPrefab, optionAction2).SetText(optionText2);
		msgBox.AddOption(messageOptionPrefab, optionAction3).SetText(optionText3);
		msgBox.AddOption(messageOptionPrefab, optionAction4).SetText(optionText4);
		return msgBox;
	}

	public MessageBox Show(string title, string message,
		string optionText1, Action optionAction1,
		string optionText2, Action optionAction2,
		string optionText3, Action optionAction3,
		string optionText4, Action optionAction4,
		string optionText5, Action optionAction5
	) {
		var msgBox = Instantiate(messageBoxPrefab);
		msgBox.SetTitle(title);
		msgBox.SetMessage(message);
		msgBox.AddOption(messageOptionPrefab, optionAction1).SetText(optionText1);
		msgBox.AddOption(messageOptionPrefab, optionAction2).SetText(optionText2);
		msgBox.AddOption(messageOptionPrefab, optionAction3).SetText(optionText3);
		msgBox.AddOption(messageOptionPrefab, optionAction4).SetText(optionText4);
		msgBox.AddOption(messageOptionPrefab, optionAction5).SetText(optionText5);
		return msgBox;
	}

	public MessageBox Show(string title, string message,
		string optionText1, Action optionAction1,
		string optionText2, Action optionAction2,
		string optionText3, Action optionAction3,
		string optionText4, Action optionAction4,
		string optionText5, Action optionAction5,
		string optionText6, Action optionAction6
	) {
		var msgBox = Instantiate(messageBoxPrefab);
		msgBox.SetTitle(title);
		msgBox.SetMessage(message);
		msgBox.AddOption(messageOptionPrefab, optionAction1).SetText(optionText1);
		msgBox.AddOption(messageOptionPrefab, optionAction2).SetText(optionText2);
		msgBox.AddOption(messageOptionPrefab, optionAction3).SetText(optionText3);
		msgBox.AddOption(messageOptionPrefab, optionAction4).SetText(optionText4);
		msgBox.AddOption(messageOptionPrefab, optionAction5).SetText(optionText5);
		msgBox.AddOption(messageOptionPrefab, optionAction6).SetText(optionText6);
		return msgBox;
	}


	public MessageBox Show(string title, string message,
		string optionText1, Action optionAction1,
		string optionText2, Action optionAction2,
		string optionText3, Action optionAction3,
		string optionText4, Action optionAction4,
		string optionText5, Action optionAction5,
		string optionText6, Action optionAction6,
		string optionText7, Action optionAction7
	) {
		var msgBox = Instantiate(messageBoxPrefab);
		msgBox.SetTitle(title);
		msgBox.SetMessage(message);
		msgBox.AddOption(messageOptionPrefab, optionAction1).SetText(optionText1);
		msgBox.AddOption(messageOptionPrefab, optionAction2).SetText(optionText2);
		msgBox.AddOption(messageOptionPrefab, optionAction3).SetText(optionText3);
		msgBox.AddOption(messageOptionPrefab, optionAction4).SetText(optionText4);
		msgBox.AddOption(messageOptionPrefab, optionAction5).SetText(optionText5);
		msgBox.AddOption(messageOptionPrefab, optionAction6).SetText(optionText6);
		msgBox.AddOption(messageOptionPrefab, optionAction7).SetText(optionText7);
		return msgBox;
	}

	public MessageBox Show(string title, string message,
		string optionText1, Action optionAction1,
		string optionText2, Action optionAction2,
		string optionText3, Action optionAction3,
		string optionText4, Action optionAction4,
		string optionText5, Action optionAction5,
		string optionText6, Action optionAction6,
		string optionText7, Action optionAction7,
		string optionText8, Action optionAction8
	) {
		var msgBox = Instantiate(messageBoxPrefab);
		msgBox.SetTitle(title);
		msgBox.SetMessage(message);
		msgBox.AddOption(messageOptionPrefab, optionAction1).SetText(optionText1);
		msgBox.AddOption(messageOptionPrefab, optionAction2).SetText(optionText2);
		msgBox.AddOption(messageOptionPrefab, optionAction3).SetText(optionText3);
		msgBox.AddOption(messageOptionPrefab, optionAction4).SetText(optionText4);
		msgBox.AddOption(messageOptionPrefab, optionAction5).SetText(optionText5);
		msgBox.AddOption(messageOptionPrefab, optionAction6).SetText(optionText6);
		msgBox.AddOption(messageOptionPrefab, optionAction7).SetText(optionText7);
		msgBox.AddOption(messageOptionPrefab, optionAction8).SetText(optionText8);
		return msgBox;
	}

}
