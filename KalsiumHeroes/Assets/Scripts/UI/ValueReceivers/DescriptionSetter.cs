
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using IHas;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class DescriptionSetter : ValueReceiver<IHasDescription, DataObject> {

	protected override void ReceiveValue(IHasDescription obj) {
		GetComponent<TMPro.TMP_Text>().text = obj.description ?? $"{obj.GetType().Name} {nameof(IHasDescription)}.{nameof(IHasDescription.description)}";
	}

	protected override void ReceiveValue(DataObject obj) {
		if (obj.data is IHasDescription hasDescription) {
			GetComponent<TMPro.TMP_Text>().text = hasDescription.description ?? $"{obj.GetType().Name}.data..{nameof(IHasDescription.description)}";
		}
	}

}