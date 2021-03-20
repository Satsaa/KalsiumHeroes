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
using System.Linq;

[RequireComponent(typeof(RectTransform))]
public class Popups : Singleton<Popups> {

	public static RectTransform rectTransform => instance.transform as RectTransform;

	[Tooltip("Pressing this key invokes the cancel option, if any.")]
	public KeyCode cancelKey = KeyCode.Escape;

	[Tooltip("Pressing this key invokes the submit option, if any.")]
	public KeyCode submitKey = KeyCode.Return;

	[field: SerializeField, Tooltip("Used as the default popup.")]
	public PopupPreset defaultPopup { get; private set; }

	[HideInInspector] public List<Popup> popups = new List<Popup>();

	protected void Update() {
		if (popups.Any()) {
			var popup = popups.Last();
			if (Input.GetKeyDown(cancelKey)) {
				var option = popup.options.FirstOrDefault(v => v.button.interactable && v.button.isActiveAndEnabled && v.isActiveAndEnabled && v.flags.HasFlag(PopupOption.Flags.Cancel));
				if (option) option.Invoke();
			}
		}
	}

	/// <summary> Shows the default messagebox </summary>
	public static Popup ShowPopup(string title, string message) {
		var res = Popups.instance.defaultPopup.Show(title, message);
		return res;
	}
}
