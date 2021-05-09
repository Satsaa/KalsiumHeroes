
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using IHas;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TicksFillSetter : ValueHooker<StatusData, Status>, IOnAnimationEventEnd {

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
		GetComponent<Image>().fillAmount = data.ticks.enabled ? (float)data.ticks.value / (float)data.ticks.other : 1;
	}

	public void OnAnimationEventEnd() => UpdateValue(target.data);

}