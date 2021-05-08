
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using IHas;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class DisplayNameSetter : ValueReceiver<IHasDisplayName, DataObject> {

	protected override void ReceiveValue(IHasDisplayName obj) {
		GetComponent<TMPro.TMP_Text>().text = obj.displayName ?? $"{obj.GetType().Name} {nameof(IHasDisplayName)}.{nameof(IHasDisplayName.displayName)}";
	}

	protected override void ReceiveValue(DataObject obj) {
		if (obj.data is IHasDisplayName hasDisplayName) {
			GetComponent<TMPro.TMP_Text>().text = hasDisplayName.displayName ?? $"{obj.GetType().Name}.data.{nameof(IHasDisplayName.displayName)}";
		}
	}

}