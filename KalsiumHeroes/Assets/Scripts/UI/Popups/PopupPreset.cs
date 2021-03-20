using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Muc.Data;
using UnityEngine.UI;
using System;
using TMPro;
using Object = UnityEngine.Object;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = nameof(PopupPreset), menuName = "KalsiumHeroes/" + nameof(PopupPreset))]
public class PopupPreset : ScriptableObject {

	[SerializeField] Popup popupPrefab;
	[SerializeField] PopupOption optionPrefab;

	[SerializeField] TextSource baseTitle;
	[SerializeField] TextSource baseMessage;
	[SerializeField] TextSource baseOptionText;

	public struct Option {
		public string text;
		public Action action;
		public PopupOption.Flags key;

		public Option(string text, Action action, PopupOption.Flags key = 0) {
			this.text = text;
			this.action = action;
			this.key = key;
		}
	}

	protected virtual void DoTitle(Popup msgBox, string title) {
		msgBox.SetTitle(title);
	}

	protected virtual void DoMessage(Popup msgBox, string message) {
		msgBox.SetMessage(message);
	}

	protected virtual void DoCustom(Popup msgBox) {

	}

	protected virtual void DoOption(Popup msgBox, Option option) {
		var opt = msgBox.AddOption(optionPrefab, option.action);
		opt.SetText(option.text);
		opt.flags = option.key;
	}


	public Popup Show(string message) => Show(null, message);
	public Popup Show(string title, string message) {
		return Show(title, message, new Option(baseOptionText, null, PopupOption.Flags.Cancel | PopupOption.Flags.Default));
	}

	public Popup Show(string message, params Option[] options) => Show(null, message, options);
	public Popup Show(string title, string message, params Option[] options) {
		var msgBox = Instantiate(popupPrefab);
		msgBox.gameObject.transform.SetParent(Popups.rectTransform);
		DoTitle(msgBox, title);
		DoMessage(msgBox, message);
		DoCustom(msgBox);
		foreach (var option in options) {
			DoOption(msgBox, option);
		}
		return msgBox;
	}

}
