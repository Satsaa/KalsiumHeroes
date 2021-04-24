
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class LoreSetter : ValueReceiver<UnitData> {

	protected override void ReceiveValue(UnitData data) {
		GetComponent<TMPro.TMP_Text>().text = data.lore;
	}

}