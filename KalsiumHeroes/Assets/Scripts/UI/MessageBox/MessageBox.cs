using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Muc.Data;
using UnityEngine.UI;
using System;
using TMPro;

public class MessageBox : MonoBehaviour {

	[SerializeField] private TMP_Text title;
	[SerializeField] private TMP_Text message;
	[SerializeField] private RectTransform options;

	protected void Start() {
		MessageBoxes.MoveToTop(this);
		MessageBoxes.IncreaseShade();
	}

	protected void OnDestroy() {
		MessageBoxes.DecreaseShade();
	}

	public virtual void SetTitle(string title) => this.title.text = title;
	public virtual void SetMessage(string message) => this.message.text = message;

	public virtual MessageOption AddOption(MessageOption option, Action action) {
		var mo = Instantiate(option, Vector3.zero, Quaternion.identity, options);
		mo.AddAction(action);
		return mo;
	}

}
