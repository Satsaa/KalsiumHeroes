using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Muc.Data;
using UnityEngine.UI;
using TMPro;

public class MessageOption : MonoBehaviour {

	[SerializeField] Button button;
	[SerializeField] TMP_Text text;

	protected void Awake() {
		if (!button) Debug.Assert(button = GetComponentInChildren<Button>());
		if (!text) Debug.Assert(text = GetComponentInChildren<TMP_Text>());
		button.onClick.AddListener(() => GetComponentInParent<MessageBox>().GetComponent<Animator>().Play("Hide"));
	}

	public virtual void SetInteractable(bool interactable) {
		button.interactable = interactable;
	}

	public virtual void SetText(string text) {
		this.text.text = text;
	}

	public virtual void AddAction(System.Action action) {
		if (action != null) button.onClick.AddListener(() => action());
	}
}
