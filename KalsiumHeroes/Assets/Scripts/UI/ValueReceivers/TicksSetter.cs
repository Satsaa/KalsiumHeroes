
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class TicksSetter : ValueReceiver<StatusData> {

	[SerializeField, Tooltip("0 is current tick number, 1 is max tick number. \"{0}/{1}\" -> \"5/10\"")]
	string format = "{0}/{1}";

	protected override void ReceiveValue(StatusData data) {
		GetComponent<TMPro.TMP_Text>().text = String.Format(format, data.ticks.value, data.ticks.other);
	}

}