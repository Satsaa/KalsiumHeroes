
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class DispellableSetter : ValueHooker<StatusData, Status>, IOnAnimationEventEnd {

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
	protected Toggle comp;

	protected void UpdateValue(StatusData data) {
		if (!comp) comp = GetComponent<Toggle>();
		comp.isOn = data.dispellable.value;
	}

	public void OnAnimationEventEnd() => UpdateValue(target.data);

}