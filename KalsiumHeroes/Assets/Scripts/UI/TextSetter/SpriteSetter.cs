
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SpriteSetter : ValueReceiver<PassiveData, AbilityData, UnitModifierData> {

	protected override void ReceiveValue(PassiveData data) {
		GetComponent<Image>().sprite = data.sprite;
	}

	protected override void ReceiveValue(AbilityData data) {
		GetComponent<Image>().sprite = data.sprite;
	}

	protected override void ReceiveValue(UnitModifierData data) {
		GetComponent<Image>().sprite = data.sprite;
	}

}