
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.Events;

public class IntReader : DataFieldReader<int> {

	[SerializeField, Tooltip("Formatting string for the text.\n{0} = current value\n{1} = max value\n{2} = percent full")]
	string enabledFormat = "{0}/{1}";

	[SerializeField, Tooltip("Formatting string for the text.\n{0} = current value\n{1} = max value\n{2} = percent full")]
	string disabledFormat = "∞";

	[SerializeField, Tooltip("Invoked when the attribute changes in any way or the reader is initialized. The formatted string based on `format` is returned.")]
	UnityEvent<string> onUpdate;

	protected override void OnReceive() => Handle();
	protected override void OnValue(int value) => Handle();
	protected override void OnOther(int value) => Handle();
	protected override void OnEnabled(bool enabled) => Handle();

	protected virtual void Handle() {
		var current = (float)selector.GetValue(data);
		var maxValue = (float)selector.GetOther(data);
		var enabled = selector.GetEnabled(data);
		onUpdate.Invoke(String.Format(enabled ? enabledFormat : disabledFormat, current, maxValue, current / maxValue * 100));
	}

}