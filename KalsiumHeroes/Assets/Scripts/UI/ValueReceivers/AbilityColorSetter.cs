
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.UI;
using Muc.Data;

[RequireComponent(typeof(Graphic))]
public class AbilityColorSetter : ValueReceiver<AbilityData, PassiveData, Ability, Passive> {

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

	protected override void ReceiveValue(Ability obj) {
		if (colors.TryGetValue(obj.data.abilityType, out var color)) {
			GetComponent<Graphic>().color = color;
		}
	}

	protected override void ReceiveValue(Passive obj) {
		GetComponent<Graphic>().color = passiveColor;
	}

}