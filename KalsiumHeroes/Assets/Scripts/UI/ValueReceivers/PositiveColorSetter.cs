
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using IHas;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
public class PositiveColorSetter : ValueHooker<StatusData, Status>, IOnAnimationEventEnd {

	[SerializeField] Color positiveColor = Color.green;
	[SerializeField] Color negativeColor = Color.red;

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

	protected void UpdateValue(StatusData data) {
		GetComponent<Graphic>().color = data.positive.value ? positiveColor : negativeColor;
	}

	public void OnAnimationEventEnd() => UpdateValue(target.data);

}