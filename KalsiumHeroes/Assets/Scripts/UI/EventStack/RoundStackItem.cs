
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoundStackItem : EventStackItem {

	public Text hint;

	public void Init(int round) {
		hint.text = $"Round {round}";
	}

	public override void OnReachZero(EventStack es) {
		es.Remove(0);
		Destroy(gameObject);
	}
}