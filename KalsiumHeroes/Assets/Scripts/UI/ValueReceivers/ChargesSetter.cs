
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class ChargesSetter : ValueReceiver<AbilityData> {

	[SerializeField, Tooltip("Formatting for the value text. 0 = current charges, 1 = max charges. \"{0}/{1}\" -> \"5/10\"")]
	string format = "{0}/{1}";

	protected override void ReceiveValue(AbilityData data) {
		GetComponent<TMPro.TMP_Text>().text = String.Format(format, data.charges.value, data.charges.other);
	}

}