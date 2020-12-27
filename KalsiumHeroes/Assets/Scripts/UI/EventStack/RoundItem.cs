
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoundItem : EventStackItem {

	public Text hint;
	public int round;
	[HideInInspector] public bool removed;

	void Start() {
		hint.text = $"Round {round}";
		removed = false;
	}

	new void Update() {
		if (transform.localPosition.x <= 3) {
			Destroy(gameObject);
		} else {
			base.Update();
		}
	}

}