
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class HiddenSetter : ValueHooker<StatusData, Status>, IOnAnimationEventEnd {

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
	protected Toggle comp;

	protected void UpdateValue(StatusData data) {
		if (!comp) comp = GetComponent<Toggle>();
		comp.isOn = data.hidden.value;
	}

	public void OnAnimationEventEnd() => UpdateValue(target.data);

}