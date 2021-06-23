
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.Events;
using Muc.Data;

public class NumericConverter<TParam> : NumericDataFieldReader {

	[SerializeField] bool selectClosest;
	[SerializeField] TParam defaultParam;
	[SerializeField] SerializedDictionary<float, TParam> parameters;

	[SerializeField, Tooltip("Invoked when the attribute changes in any way or the reader is initialized.")]
	UnityEvent<TParam> onUpdate;

	protected override void OnReceive() => Handle();
	protected override void OnValue(float value) => Handle();
	protected override void OnOther(float value) => Handle();
	protected override void OnEnabled(bool enabled) => Handle();

	protected virtual void Handle() {
		var current = selector.GetValue(data);
		if (parameters.TryGetValue(current, out var parameter)) {
			onUpdate.Invoke(parameter);
		} else {
			if (selectClosest) {
				var closest = defaultParam;
				var closestDist = float.PositiveInfinity;
				foreach (var kv in parameters) {
					var dist = Mathf.Abs(kv.Key - current);
					if (dist < closestDist) {
						closest = kv.Value;
						closestDist = dist;
					}
				}
				onUpdate.Invoke(closest);
				return;
			}
			onUpdate.Invoke(defaultParam);
		}
	}

}