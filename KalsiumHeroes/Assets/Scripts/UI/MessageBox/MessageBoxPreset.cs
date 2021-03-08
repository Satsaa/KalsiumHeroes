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

	protected virtual void DoTitle(MessageBox msgBox, string title) {
		msgBox.SetTitle(title);
	}

	protected virtual void DoMessage(MessageBox msgBox, string message) {
		msgBox.SetMessage(message);
	}

	protected virtual void DoCustom(MessageBox msgBox) {

	}

	protected virtual void DoOption(MessageBox msgBox, Action action, string text) {
		msgBox.AddOption(messageOptionPrefab, action).SetText(text);
	}


	public MessageBox Show(string title = null, string message = null
	) {
		var msgBox = Instantiate(messageBoxPrefab);
		DoTitle(msgBox, title ?? baseTitle);
		DoMessage(msgBox, message ?? baseMessage);
		DoCustom(msgBox);
		DoOption(msgBox, null, baseOptionText);
		return msgBox;
	}

	public MessageBox Show(string title, string message,
		string optionText1, Action optionAction1
	) {
		var msgBox = Instantiate(messageBoxPrefab);
		DoTitle(msgBox, title);
		DoMessage(msgBox, message);
		DoCustom(msgBox);
		DoOption(msgBox, optionAction1, optionText1);
		return msgBox;
	}

	public MessageBox Show(string title, string message,
		string optionText1, Action optionAction1,
		string optionText2, Action optionAction2
	) {
		var msgBox = Instantiate(messageBoxPrefab);
		DoTitle(msgBox, title);
		DoMessage(msgBox, message);
		DoCustom(msgBox);
		DoOption(msgBox, optionAction1, optionText1);
		DoOption(msgBox, optionAction2, optionText2);
		return msgBox;
	}

	public MessageBox Show(string title, string message,
		string optionText1, Action optionAction1,
		string optionText2, Action optionAction2,
		string optionText3, Action optionAction3
	) {
		var msgBox = Instantiate(messageBoxPrefab);
		DoTitle(msgBox, title);
		DoMessage(msgBox, message);
		DoCustom(msgBox);
		DoOption(msgBox, optionAction1, optionText1);
		DoOption(msgBox, optionAction2, optionText2);
		DoOption(msgBox, optionAction3, optionText3);
		return msgBox;
	}

	public MessageBox Show(string title, string message,
		string optionText1, Action optionAction1,
		string optionText2, Action optionAction2,
		string optionText3, Action optionAction3,
		string optionText4, Action optionAction4
	) {
		var msgBox = Instantiate(messageBoxPrefab);
		DoTitle(msgBox, title);
		DoMessage(msgBox, message);
		DoCustom(msgBox);
		DoOption(msgBox, optionAction1, optionText1);
		DoOption(msgBox, optionAction2, optionText2);
		DoOption(msgBox, optionAction3, optionText3);
		DoOption(msgBox, optionAction4, optionText4);
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
		DoTitle(msgBox, title);
		DoMessage(msgBox, message);
		DoCustom(msgBox);
		DoOption(msgBox, optionAction1, optionText1);
		DoOption(msgBox, optionAction2, optionText2);
		DoOption(msgBox, optionAction3, optionText3);
		DoOption(msgBox, optionAction4, optionText4);
		DoOption(msgBox, optionAction5, optionText5);
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
		DoTitle(msgBox, title);
		DoMessage(msgBox, message);
		DoCustom(msgBox);
		DoOption(msgBox, optionAction1, optionText1);
		DoOption(msgBox, optionAction2, optionText2);
		DoOption(msgBox, optionAction3, optionText3);
		DoOption(msgBox, optionAction4, optionText4);
		DoOption(msgBox, optionAction5, optionText5);
		DoOption(msgBox, optionAction6, optionText6);
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
		DoTitle(msgBox, title);
		DoMessage(msgBox, message);
		DoCustom(msgBox);
		DoOption(msgBox, optionAction1, optionText1);
		DoOption(msgBox, optionAction2, optionText2);
		DoOption(msgBox, optionAction3, optionText3);
		DoOption(msgBox, optionAction4, optionText4);
		DoOption(msgBox, optionAction5, optionText5);
		DoOption(msgBox, optionAction6, optionText6);
		DoOption(msgBox, optionAction7, optionText7);
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
		DoTitle(msgBox, title);
		DoMessage(msgBox, message);
		DoCustom(msgBox);
		DoOption(msgBox, optionAction1, optionText1);
		DoOption(msgBox, optionAction2, optionText2);
		DoOption(msgBox, optionAction3, optionText3);
		DoOption(msgBox, optionAction4, optionText4);
		DoOption(msgBox, optionAction5, optionText5);
		DoOption(msgBox, optionAction6, optionText6);
		DoOption(msgBox, optionAction7, optionText7);
		DoOption(msgBox, optionAction8, optionText8);
		return msgBox;
	}

}
