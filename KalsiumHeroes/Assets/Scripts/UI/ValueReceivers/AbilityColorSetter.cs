
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Data;
using UnityEngine.Events;

public class AbilityColorSetter : ValueReceiver<AbilityData, PassiveData, Ability, Passive> {

	[SerializeField] Color passiveColor;
	[SerializeField] SerializedDictionary<AbilityType, Color> colors;

	[SerializeField] UnityEvent<Color> onUpdate;

	protected override void ReceiveValue(AbilityData data) {
		if (colors.TryGetValue(data.abilityType, out var color)) {
			onUpdate.Invoke(color);
		} else {
			onUpdate.Invoke(Color.white);
		}
	}

	protected override void ReceiveValue(PassiveData data) {
		onUpdate.Invoke(passiveColor);
	}

	protected override void ReceiveValue(Ability obj) {
		ReceiveValue(obj.data);
	}

	protected override void ReceiveValue(Passive obj) {
		ReceiveValue(obj.data);
	}

}