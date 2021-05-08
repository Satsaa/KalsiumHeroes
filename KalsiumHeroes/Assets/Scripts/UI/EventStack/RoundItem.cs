
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoundItem : EventStackItem {

	public int round;
	[HideInInspector] public bool removed;

	void Start() {
		removed = false;
		ValueReceiver.SendValue(gameObject, round);
	}

	new void Update() {
		if (transform.localPosition.x <= 3) {
			Destroy(gameObject);
		} else {
			base.Update();
		}
	}

}