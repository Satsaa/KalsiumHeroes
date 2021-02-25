using System.Collections;
using System.Collections.Generic;
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
	[SerializeField] private RectTransform options;

	protected void Start() {
		Windows.MoveToTop(transform);
		transform.localPosition = default;
	}

	public virtual void SetTitle(string title) => this.title.text = title;
	public virtual void SetMessage(string message) => this.message.text = message;

	/// <summary> Adds the Object after the message UI element </summary>
	public virtual void AddCustomObject(GameObject go) {
		go.transform.SetParent(message.transform.parent);
		go.transform.localPosition = default;
		go.transform.SetSiblingIndex(2);
	}

	public virtual MessageOption AddOption(MessageOption optionPrefab, Action action) {
		var option = Instantiate(optionPrefab, Vector3.zero, Quaternion.identity, options);
		option.AddAction(action);
		return option;
	}

}
