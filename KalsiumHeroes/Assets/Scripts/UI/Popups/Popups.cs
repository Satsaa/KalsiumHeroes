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

[RequireComponent(typeof(RectTransform))]
public class Popups : Singleton<Popups> {

	public static RectTransform rectTransform => instance.transform as RectTransform;
	[SerializeField] private PopupPreset defaultPopup;

	/// <summary> Shows the default messagebox </summary>
	public static Popup ShowPopup(string title, string message) {
		var res = Popups.instance.defaultPopup.Show(title, message);
		return res;
	}
}
