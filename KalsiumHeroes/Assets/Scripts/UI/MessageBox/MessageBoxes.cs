using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Muc.Data;
using UnityEngine.UI;

public class MessageBoxes : Singleton<MessageBoxes> {

	[SerializeField] private Image _shade;
	[SerializeField, Range(0, 1)] private float shadeAmount;
	[SerializeField, HideInInspector] private int _currentShadeCount;

	public static void MoveToTop(MessageBox messageBox) {
		Windows.MoveToTop(instance._shade.transform);
		Windows.MoveToTop(messageBox.transform);
	}

	public static void IncreaseShade() {
		var alpha = Mathf.Pow(instance.shadeAmount, instance._currentShadeCount);
		instance._shade.color = new Color(instance._shade.color.r, instance._shade.color.g, instance._shade.color.b, alpha);
		instance._shade.enabled = true;
	}

	public static void DecreaseShade() {
		instance._currentShadeCount = Mathf.Max(0, instance._currentShadeCount - 1);
		if (instance._currentShadeCount == 0) {
			instance._shade.enabled = false;
		} else {
			var alpha = Mathf.Pow(instance.shadeAmount, instance._currentShadeCount);
			instance._shade.color = new Color(instance._shade.color.r, instance._shade.color.g, instance._shade.color.b, alpha);
			instance._shade.enabled = true;
		}
	}
}
