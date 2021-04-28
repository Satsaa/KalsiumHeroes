
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class ChargesSetter : ValueHooker<AbilityData, Ability>, IOnAbilityCastStart_Unit, IOnAnimationEventEnd {

	[SerializeField, Tooltip("Formatting for the value text. 0 = current charges, 1 = max charges. \"{0}/{1}\" -> \"5/10\"")]
	string format = "{0}/{1}";

	protected override void ReceiveValue(AbilityData data) {
		UpdateValue(data);
	}

	protected override void ReceiveValue(Ability target) {
		this.target = target;
		UpdateValue(target.data);
		Hook(target.unit);
	}


	[SerializeField, HideInInspector]
	protected Ability target;
	protected TMPro.TMP_Text comp;

	protected void UpdateValue(AbilityData data) {
		if (!comp) comp = GetComponent<TMPro.TMP_Text>();
		comp.text = String.Format(format, data.charges.value, data.charges.other);
	}

	public void OnAbilityCastStart(Ability ability) { if (ability == target) UpdateValue(target.data); }
	public void OnAnimationEventEnd() => UpdateValue(target.data);

}