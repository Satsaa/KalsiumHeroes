
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using IHas;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class DisplayNameSetter : ValueReceiver<IHasDisplayName> {

	protected override void ReceiveValue(IHasDisplayName obj) {
		GetComponent<TMPro.TMP_Text>().text = obj.displayName ?? $"{obj.GetType().Name} {nameof(IHasDisplayName)}.{nameof(IHasDisplayName.displayName)}";
	}

}