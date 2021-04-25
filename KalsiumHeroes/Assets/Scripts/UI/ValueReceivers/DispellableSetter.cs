
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class DispellableSetter : ValueReceiver<StatusData> {

	protected override void ReceiveValue(StatusData data) {
		GetComponent<Toggle>().isOn = data.dispellable.value;
	}

}