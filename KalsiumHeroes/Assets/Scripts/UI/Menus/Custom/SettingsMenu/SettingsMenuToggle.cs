
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class SettingsMenuToggle : MonoBehaviour {

	public Image image;
	public GameObject tab;

	public Sprite unselected;
	public Color unselectedColor = Color.white;

	public Sprite selected;
	public Color selectedColor = Color.white;

	void Reset() {
		if (TryGetComponent<Toggle>(out var toggle)) {
			image = toggle.image;
			if (image) {
				if (toggle.isOn) {
					selected = image.sprite;
					selectedColor = image.color;
				} else {
					unselected = image.sprite;
					unselectedColor = image.color;
				}
			}
		}
	}

	void Start() {
		OnToggle(GetComponent<Toggle>().isOn);
	}

	public void OnToggle(bool value) {
		if (value) {
			image.sprite = selected;
			image.color = selectedColor;
		} else {
			image.sprite = unselected;
			image.color = unselectedColor;
		}
		tab.SetActive(value);
	}
}