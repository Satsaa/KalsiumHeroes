
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class EnergyCostSetter : ValueHooker<AbilityData, Ability>, IOnAnimationEventEnd {

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
		comp.text = data.energyCost.value.ToString();
	}

	public void OnAnimationEventEnd() => UpdateValue(target.data);

}