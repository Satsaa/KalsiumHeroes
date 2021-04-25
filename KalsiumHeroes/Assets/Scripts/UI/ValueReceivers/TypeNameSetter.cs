
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using IHas;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class LoreSetter : ValueReceiver<IHasLore> {

	protected override void ReceiveValue(IHasLore obj) {
		GetComponent<TMPro.TMP_Text>().text = obj.lore ?? $"{obj.GetType().Name}.{nameof(IHasLore.lore)}";
	}

}