
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class CooldownSetter : ValueReceiver<AbilityData> {

	[SerializeField, Tooltip("0 is current cooldown, 1 is max cooldown. \"{0}/{1}\" -> \"5/10\"")]
	string format = "{0}/{1}";

	protected override void ReceiveValue(AbilityData data) {
		GetComponent<TMPro.TMP_Text>().text = String.Format(format, data.cooldown.value, data.cooldown.other);
	}

}