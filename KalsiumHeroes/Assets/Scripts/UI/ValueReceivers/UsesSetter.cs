
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class UsesSetter : ValueHooker<AbilityData, Ability>, IOnAbilityCastStart_Unit, IOnAnimationEventEnd {

	[SerializeField, Tooltip("This will be shown if uses are disabled (no use limit)")]
	string infiniteString = "∞";

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
		comp.text = data.uses.enabled ? data.uses.value.ToString() : infiniteString;
	}

	public void OnAbilityCastStart(Ability ability) { if (ability == target) UpdateValue(target.data); }
	public void OnAnimationEventEnd() => UpdateValue(target.data);

}