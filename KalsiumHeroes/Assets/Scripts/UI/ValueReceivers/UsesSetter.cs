
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class UsesSetter : ValueReceiver<AbilityData> {

	[SerializeField, Tooltip("This will be shown if uses are disabled (no use limit)")]
	string infiniteString = "∞";

	protected override void ReceiveValue(AbilityData data) {
		GetComponent<TMPro.TMP_Text>().text = data.uses.enabled ? data.uses.value.ToString() : infiniteString;
	}

}