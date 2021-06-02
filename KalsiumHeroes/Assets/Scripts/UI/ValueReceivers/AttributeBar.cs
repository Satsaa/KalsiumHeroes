
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Extensions;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AttributeBar : ValueHooker<UnitData, Unit>, IOnAnimationEventEnd {

	[SerializeField] NumericAttributeSelector attribute;

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
			maxValue = Mathf.Max(current, maxValue);

			var fract = current / maxValue;

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

	public void OnAnimationEventEnd() => UpdateValue(target.data);

}