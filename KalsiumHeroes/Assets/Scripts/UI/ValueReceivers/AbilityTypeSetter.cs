
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class AbilityTypeSetter : ValueReceiver<AbilityData, PassiveData> {

	[SerializeField] TextSource passiveText;
	[SerializeField] SerializedDictionary<AbilityType, TextSource> translations;

	protected override void ReceiveValue(AbilityData data) {
		if (translations.TryGetValue(data.abilityType, out var translation)) {
			GetComponent<TMPro.TMP_Text>().text = translation;
		} else {
			GetComponent<TMPro.TMP_Text>().text = Enum.GetName(typeof(AbilityType), data.abilityType);
		}
	}

	protected override void ReceiveValue(PassiveData data) {
		GetComponent<TMPro.TMP_Text>().text = passiveText ?? nameof(Passive);
	}

}