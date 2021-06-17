
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.Events;

public class FullnessReader : NumericDataFieldReader {

	[SerializeField] UnityEvent<float> onUpdate;

	protected override void OnReceive() => Handle();
	protected override void OnValue(float value) => Handle();
	protected override void OnOther(float value) => Handle();
	protected override void OnEnabled(bool enabled) => Handle();

	protected virtual void Handle() {
		var value = selector.GetValue(data) / selector.GetOther(data);
		if (float.IsNaN(value)) value = 1;
		onUpdate.Invoke(value);
	}

}