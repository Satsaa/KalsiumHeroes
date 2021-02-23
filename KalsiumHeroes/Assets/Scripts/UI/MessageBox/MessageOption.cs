using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Muc.Data;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MessageOption : MonoBehaviour {

	[SerializeField, HideInInspector] Button button;

	protected void Awake() {
		button = GetComponent<Button>();
	}

	public virtual void SetEnabled(bool enabled) {
		button.interactable = enabled;
	}

	public virtual void AddAction(System.Action action) {
		button.onClick.AddListener(() => action());
	}
}
