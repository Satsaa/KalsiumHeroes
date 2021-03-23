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

	[field: SerializeField, Tooltip("Used as the default popup.")]
	public PopupPreset defaultPopup { get; private set; }

	[HideInInspector] public List<Popup> popups = new List<Popup>();

	/// <summary> Shows the default messagebox </summary>
	public static Popup ShowPopup(string title, string message) {
		var res = Popups.instance.defaultPopup.Show(title, message);
		return res;
	}

	public void TryClose() {
		if (popups.Any()) {
			var popup = popups.Last();
			var option = popup.options.FirstOrDefault(v => v.button.interactable && v.button.isActiveAndEnabled && v.isActiveAndEnabled && v.flags.HasFlag(PopupOption.Flags.Cancel));
			if (option) option.Invoke();
		}
	}

}
