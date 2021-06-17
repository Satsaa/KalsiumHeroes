
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Extensions;
using UnityEngine.Events;

public class PositivinessReader : NumericDataFieldReader {

	[SerializeField] Color neutralColor = Color.white;
	[SerializeField] Color positiveColor = Color.green;
	[SerializeField] Color negativeColor = Color.red;

	[SerializeField] UnityEvent<Color> onUpdate;

	protected override void OnReceive() => Handle();
	protected override void OnValue(float value) => Handle();
	protected override void OnOther(float value) => Handle();
	protected override void OnEnabled(bool enabled) => Handle();

	protected virtual void Handle() {

		var current = selector.GetValue(data);
		var raw = selector.GetRawValue(data);
		var sign = current.CompareTo(raw);

		onUpdate.Invoke(sign == -1 ? negativeColor : sign == 1 ? positiveColor : neutralColor);

	}

}