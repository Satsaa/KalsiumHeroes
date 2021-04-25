
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class EnergyCostSetter : ValueReceiver<AbilityData> {

	protected override void ReceiveValue(AbilityData data) {
		GetComponent<TMPro.TMP_Text>().text = data.energyCost.value.ToString();
	}

}