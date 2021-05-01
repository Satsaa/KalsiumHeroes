using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Muc.Extensions;
using UnityEngine;

public class UnitUIModifier : UnitModifier, IOnLateUpdate {

	public new UnitUIModifierData data => (UnitUIModifierData)_data;
	public override Type dataType => typeof(UnitUIModifierData);

	[SerializeField, HideInInspector] RectTransform rect;
	[SerializeField, HideInInspector] protected bool updatePosition = true;

	public virtual void OnLateUpdate() {
		if (updatePosition) {
			var pos =
				Camera.main.WorldToScreenPoint(master.transform.position + data.trackingOffset).xy() +
				data.pixelOffset +
				data.screenOffset * new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);

			rect.position = pos;

			if (data.clampToScreen) {

				var clampRect = (rect.parent as RectTransform).rect;
				clampRect = new Rect(
					clampRect.x + data.clampMin.x * clampRect.width,
					clampRect.y + data.clampMin.y * clampRect.height,
					clampRect.width * (data.clampMax.x - data.clampMin.x),
					clampRect.height * (data.clampMax.y - data.clampMin.y)
				);

				var clamp = rect.localPosition;

				var minPosition = clampRect.min - rect.rect.min;
				var maxPosition = clampRect.max - rect.rect.max;

				clamp.x = Mathf.Clamp(rect.localPosition.x, minPosition.x, maxPosition.x);
				clamp.y = Mathf.Clamp(rect.localPosition.y, minPosition.y, maxPosition.y);

				rect.localPosition = clamp;

			}

			if (data.scaleMode > 0) {

				var t = data.scaleMode switch {
					UnitUIModifierData.ScaleMode.Distance => Vector3.Distance(Camera.main.transform.position, master.transform.position + data.trackingOffset),
					UnitUIModifierData.ScaleMode.CameraHeight => Camera.main.transform.position.y,
					UnitUIModifierData.ScaleMode.HeightDifference => Camera.main.transform.position.y - (master.transform.position.y + data.trackingOffset.y),
					UnitUIModifierData.ScaleMode.TileHeightDifference => Camera.main.transform.position.y - unit.tile.center.y,
					_ => throw new ArgumentOutOfRangeException("Invalid enum value.", nameof(UnitUIModifierData.scaleMode)),
				};
				var scale = data.scaleCurve.Evaluate(t);
				container.transform.localScale = new Vector3(scale, scale, scale);

			}

		}
	}

	protected override void OnCreate() {
		rect = container.GetComponent<RectTransform>();
		if (data.sendUnit) {
			ValueReceiver.SendValue(container, unit);
		}
	}

}
