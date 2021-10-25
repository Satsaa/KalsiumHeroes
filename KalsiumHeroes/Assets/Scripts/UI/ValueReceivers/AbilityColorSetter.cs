
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Data;
using UnityEngine.Events;

public class AbilityColorSetter : ValueReceiver<Ability, Passive> {

	[SerializeField] Color passiveColor;
	[SerializeField] SerializedDictionary<AbilityType, Color> colors;

	[SerializeField] UnityEvent<Color> onUpdate;

	protected override void ReceiveValue(Ability data) {
		if (colors.TryGetValue(data.abilityType.current, out var color)) {
			onUpdate.Invoke(color);
		} else {
			onUpdate.Invoke(Color.white);
		}
	}

	protected override void ReceiveValue(Passive data) {
		onUpdate.Invoke(passiveColor);
	}

}