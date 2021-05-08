
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using IHas;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class LoreSetter : ValueReceiver<IHasLore, DataObject> {

	protected override void ReceiveValue(IHasLore obj) {
		GetComponent<TMPro.TMP_Text>().text = obj.lore ?? $"{obj.GetType().Name} {nameof(IHasLore)}.{nameof(IHasLore.lore)}";
	}

	protected override void ReceiveValue(DataObject obj) {
		if (obj.data is IHasLore hasLore) {
			GetComponent<TMPro.TMP_Text>().text = hasLore.lore ?? $"{obj.GetType().Name}.data.{nameof(IHasLore.lore)}";
		}
	}

}