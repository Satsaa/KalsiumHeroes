
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

		var correction = 0f;
		if (image.type is Image.Type.Sliced or Image.Type.Tiled && image.hasBorder) {
			var pixels = (image.sprite.border.x * (1 - fract) * (1 / image.pixelsPerUnitMultiplier));
			correction += pixels / (rt.rect.width * ((rt.anchorMax.x - rt.anchorMin.x) * -1 + 2));
			correction *= 2;
		}
		rt.anchorMin = rt.anchorMin.SetX(0);
		rt.anchorMax = rt.anchorMax.SetX(fract + correction);

	}

}