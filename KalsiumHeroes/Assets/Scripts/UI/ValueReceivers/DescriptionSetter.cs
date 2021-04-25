
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using IHas;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class DescriptionSetter : ValueReceiver<IHasDescription> {

	protected override void ReceiveValue(IHasDescription obj) {
		GetComponent<TMPro.TMP_Text>().text = obj.description ?? $"{obj.GetType().Name} {nameof(IHasDescription)}.{nameof(IHasDescription.description)}";
	}

}