
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class TickModeSetter : ValueReceiver<StatusData> {

	protected override void ReceiveValue(StatusData data) {
		GetComponent<TMPro.TMP_Text>().text = typeof(TickMode).GetEnumName(data.tickMode);
	}

}