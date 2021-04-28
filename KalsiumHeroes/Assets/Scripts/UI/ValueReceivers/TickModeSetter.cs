
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class TickModeSetter : ValueHooker<StatusData, Status>, IOnAnimationEventEnd {

	protected override void ReceiveValue(StatusData data) {
		UpdateValue(data);
	}

	protected override void ReceiveValue(Status status) {
		target = status;
		UpdateValue(status.data);
		Hook(status.unit);
	}


	[SerializeField, HideInInspector]
	protected Status target;
	protected TMPro.TMP_Text comp;

	protected void UpdateValue(StatusData data) {
		if (!comp) comp = GetComponent<TMPro.TMP_Text>();
		comp.text = typeof(TickMode).GetEnumName(data.tickMode);
	}

	public void OnAnimationEventEnd() => UpdateValue(target.data);

}