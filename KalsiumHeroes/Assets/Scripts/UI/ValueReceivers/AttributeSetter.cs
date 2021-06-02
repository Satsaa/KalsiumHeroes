
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Extensions;
using UnityEngine.UI;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class AttributeSetter : ValueHooker<UnitData, Unit>, IOnAnimationEventEnd {

	[SerializeField] NumericAttributeSelector attribute;

	[SerializeField, Tooltip("Formatting string for the text. {0} = current value, {1} = max value, {2} = percent full.")]
	string format = "{0}/{1}";

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
			var maxValue = attribute.GetOther(data);

			GetComponent<TMPro.TMP_Text>().text = String.Format(format, current, maxValue, current / maxValue * 100);

		}

	}

	public void OnAnimationEventEnd() => UpdateValue(target.data);

}