
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.UI;
using Muc.Data;

[RequireComponent(typeof(Graphic))]
public class AbilityColorSetter : ValueReceiver<AbilityData, PassiveData> {

	[SerializeField] Color passiveColor;
	[SerializeField] SerializedDictionary<AbilityType, Color> colors;

	protected override void ReceiveValue(AbilityData data) {
		if (colors.TryGetValue(data.abilityType, out var color)) {
			GetComponent<Graphic>().color = color;
		}
	}

	protected override void ReceiveValue(PassiveData data) {
		GetComponent<Graphic>().color = passiveColor;
	}

}