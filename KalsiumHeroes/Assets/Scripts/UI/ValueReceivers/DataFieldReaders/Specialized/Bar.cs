
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Extensions;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Bar : NumericDataFieldReader {

	protected override void OnReceive() => Handle();
	protected override void OnValue(float value) => Handle();
	protected override void OnOther(float value) => Handle();
	protected override void OnEnabled(bool enabled) => Handle();

	protected virtual void Handle() {

		var value = selector.GetValue(data);
		var maxValue = selector.GetOther(data);

		maxValue = Mathf.Max(value, maxValue);

		var fract = value / maxValue;

		var image = GetComponent<Image>();

		image.gameObject.SetActive(fract > 0);
		RectTransform rt = image.rectTransform;

		rt.anchorMin = rt.anchorMin.SetX(0);
		rt.anchorMax = rt.anchorMax.SetX(fract);

	}

}