
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class CooldownSetter : ValueHooker<AbilityData, Ability>, IOnTurnStart_Unit, IOnTurnEnd_Unit, IOnAbilityCastStart_Unit, IOnAnimationEventEnd {

	[SerializeField, Tooltip("Formatting string for the text. {0} = current cooldown, {1} = max cooldown.")]
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
		comp.text = String.Format(format, data.cooldown.value, data.cooldown.other);
	}

	public void OnTurnStart() => UpdateValue(target.data);
	public void OnTurnEnd() => UpdateValue(target.data);
	public void OnAbilityCastStart(Ability ability) { if (ability == target) UpdateValue(target.data); }
	public void OnAnimationEventEnd() => UpdateValue(target.data);

}