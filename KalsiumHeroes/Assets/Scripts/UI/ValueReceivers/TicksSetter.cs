
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class TicksSetter : ValueHooker<StatusData, Status>, IOnTurnStart_Unit, IOnTurnEnd_Unit, IOnAnimationEventEnd {

	[SerializeField, Tooltip("Formatting string for the text. {0} = current ticks, {1} = max ticks.")]
	string format = "{0}/{1}";

	protected override void ReceiveValue(StatusData data) {
		UpdateValue(data);
	}

	protected override void ReceiveValue(Status target) {
		this.target = target;
		UpdateValue(target.data);
		Hook(target.unit);
	}


	[SerializeField, HideInInspector]
	protected Status target;
	protected TMPro.TMP_Text comp;

	protected void UpdateValue(StatusData data) {
		if (!comp) comp = GetComponent<TMPro.TMP_Text>();
		comp.text = String.Format(format, data.ticks.value, data.ticks.other);
	}

	public void OnTurnStart() => UpdateValue(target.data);
	public void OnTurnEnd() => UpdateValue(target.data);
	public void OnAnimationEventEnd() => UpdateValue(target.data);

}