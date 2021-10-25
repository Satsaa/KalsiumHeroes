
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Extensions;
using UnityEngine.Events;
using UnityEngine.UI;

public class AverageComparedBar : NumericDataFieldReader {

	[SerializeField, Tooltip("Formatting string for the value text.\n{0} = current value,\n{1} = max value,\n{2} = average value,\n{3} = linear difference from average,\n{4} = percentage difference from average.")]
	string enabledFormat = "{0}";

	[SerializeField, Tooltip("Formatting string for the value text.\n{0} = current value,\n{1} = max value,\n{2} = average value,\n{3} = linear difference from average,\n{4} = percentage difference from average.")]
	string disabledFormat = "-";

	[SerializeField, Tooltip("Invoked when the attribute changes in any way or the reader is initialized. The formatted string based on `format` is returned.")]
	UnityEvent<string> onUpdate;

	[SerializeField] float maxValue;
	[SerializeField] Image baseFiller;
	[SerializeField] Image underFiller;
	[SerializeField] Image overFiller;

	protected override void OnReceive() => Handle();
	protected override void OnValue(float value) => Handle();
	protected override void OnOther(float value) => Handle();
	protected override void OnEnabled(bool enabled) => Handle();

	protected virtual void Handle() {

		var value = selector.GetValue(data);
		var enabled = selector.GetEnabled(data);

		var average = App.library.GetByType<Unit>().Average(v => selector.GetValue(v));

		var _0 = value;
		var _1 = maxValue;
		var _2 = average;
		var _3 = value - average;
		var _4 = (value - average) / average * 100; // Difference from average as percentage
		onUpdate.Invoke(String.Format(enabled ? enabledFormat : disabledFormat, _0, _1, _2, _3, _4));

		if (baseFiller) {
			var fract = value / maxValue;
			baseFiller.gameObject.SetActive(fract > 0);
			RectTransform rt = baseFiller.rectTransform;
			rt.anchorMin = rt.anchorMin.SetX(0);
			rt.anchorMax = rt.anchorMax.SetX(fract);

		}

		var averageFract = average / maxValue;

		if (underFiller) {
			var fract = (average - value) / maxValue;
			underFiller.gameObject.SetActive(fract > 0);
			RectTransform rt = underFiller.rectTransform;
			rt.anchorMin = rt.anchorMin.SetX(averageFract - fract);
			rt.anchorMax = rt.anchorMax.SetX(averageFract);
		}

		if (overFiller) {
			var fract = (value - average) / maxValue;
			overFiller.gameObject.SetActive(fract > 0);
			RectTransform rt = overFiller.rectTransform;
			rt.anchorMin = rt.anchorMin.SetX(averageFract);
			rt.anchorMax = rt.anchorMax.SetX(averageFract + fract);
		}

	}

}