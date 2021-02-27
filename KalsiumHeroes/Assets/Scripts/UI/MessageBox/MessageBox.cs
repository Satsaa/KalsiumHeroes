using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using Muc.Data;
using UnityEngine.UI;
using System;
using TMPro;
using Object = UnityEngine.Object;

public class MessageBox : MonoBehaviour {

	[SerializeField] private TMP_Text title;
	[SerializeField] private TMP_Text message;
	[SerializeField] private RectTransform optionsParent;
	[SerializeField] private List<MessageOption> _options;
	public ReadOnlyCollection<MessageOption> options => _options.AsReadOnly();

	protected void Start() {
		Windows.MoveToTop(transform);
		transform.localPosition = default;
	}

	public virtual void SetTitle(string title) => this.title.text = title;
	public virtual void SetMessage(string message) => this.message.text = message;

	/// <summary> Adds the Object after the message UI element </summary>
	public virtual void AddCustomObject(GameObject go) {
		go.transform.SetParent(message.transform.parent);
		go.transform.SetSiblingIndex(2);
	}

	public virtual MessageOption AddOption(MessageOption optionPrefab, Action action = null) {
		var option = Instantiate(optionPrefab, Vector3.zero, Quaternion.identity, optionsParent);
		option.AddAction(action);
		_options.Add(option);
		return option;
	}

}
