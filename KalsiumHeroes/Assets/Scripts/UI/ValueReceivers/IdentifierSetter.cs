
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class IdentifierSetter : ValueReceiver<DataObjectData> {

	protected override void ReceiveValue(DataObjectData obj) {
		GetComponent<TMPro.TMP_Text>().text = obj.identifier ?? $"{nameof(DataObjectData)}.{nameof(DataObjectData.identifier)}";
	}

}