
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class DescriptionSetter : ValueReceiver<DetailsObjectData> {

	protected override void ReceiveValue(DetailsObjectData data) {
		GetComponent<TMPro.TMP_Text>().text = data.description ?? $"{data.GetType().Name}.{nameof(DetailsObjectData.description)}";
	}

}