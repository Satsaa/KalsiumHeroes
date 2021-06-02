
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Extensions;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
public class AttributePositivinessSetter : ValueHooker<UnitData, Unit>, IOnAnimationEventEnd {

	[SerializeField] NumericAttributeSelector attribute;

	[SerializeField] Color neutralColor = Color.white;
	[SerializeField] Color positiveColor = Color.green;
	[SerializeField] Color negativeColor = Color.red;

	protected override void ReceiveValue(UnitData data) {
		UpdateValue(data);
	}

	protected override void ReceiveValue(Unit target) {
		this.target = target;
		UpdateValue(target.data);
		Hook(target);
	}


	[SerializeField, HideInInspector]
	protected Unit target;

	protected void UpdateValue(UnitData data) {

		if (data) {

			var current = attribute.GetValue(data);
			var raw = attribute.GetRawValue(data);
			var sign = current.CompareTo(raw);

			GetComponent<Graphic>().color = sign == -1 ? negativeColor : sign == 1 ? positiveColor : neutralColor;

		}

	}

	public void OnAnimationEventEnd() => UpdateValue(target.data);

}