using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Muc.Extensions;
using UnityEngine;

public class UnitUIModifier : UnitModifier, IOnLateUpdate {

	[Tooltip("World space offset of the tracked object.")]
	public Vector3 trackingOffset;

	[Tooltip("Offset of the UI/container relative to the size of the screen. A (1,1) value will offset the UI 1080 units right and 1920 units up on a 1080p window.")]
	public Vector2 screenOffset;

	[Tooltip("Offset of the UI/container in pixels (affected by scaling).")]
	public Vector2Int pixelOffset;

	[Tooltip("Send the Unit object as a value to the " + nameof(ValueReceiver) + "s in the container.")]
	public bool sendUnit = true;

	[Space(5)]
	[Tooltip("Clamp the container RectTransform inside the screen.")]
	public bool clampToScreen;

	[Tooltip("Clamp area. (0,0),(1,1) = whole screen, (0.5,0.5),(1,1) = quarter of the screen.")]
	public Vector2 clampMin = new(0, 0);
	[Tooltip("Clamp area. (0,0),(1,1) = whole screen, (0.5,0.5),(1,1) = quarter of the screen.")]
	public Vector2 clampMax = new(1, 1);

	[Space(5)]
	[Tooltip("Scale the container based on distance to the camera.")]
	public ScaleMode scaleMode;

	[Tooltip("Scale curve. Note that time is not normalized but based on linear distance.")]
	public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(25, 1, 50, 0.1f);

	public enum ScaleMode {
		[Tooltip("No scaling!?!?")]
		None,
		[Tooltip("Scale based on the distance of the camera from the tracking position.")]
		Distance,
		[Tooltip("Scale based on the height of the camera.")]
		CameraHeight,
		[Tooltip("Scale based on the height difference of the camera and the Unit.")]
		HeightDifference,
		[Tooltip("Scale based on the height difference of the camera and the Tile.")]
		TileHeightDifference,
	}


	[SerializeField, HideInInspector] RectTransform rect;
	[SerializeField, HideInInspector] protected bool updatePosition = true;

	public virtual void OnLateUpdate() {
		if (updatePosition && shown && master.shown) {
			var pos =
				Camera.main.WorldToScreenPoint(master.transform.position + trackingOffset).xy() +
				pixelOffset +
				screenOffset * new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);

			rect.position = pos;

			if (clampToScreen) {

				var clampRect = ((RectTransform)rect.parent).rect;
				clampRect = new Rect(
					clampRect.x + clampMin.x * clampRect.width,
					clampRect.y + clampMin.y * clampRect.height,
					clampRect.width * (clampMax.x - clampMin.x),
					clampRect.height * (clampMax.y - clampMin.y)
				);

				var clamp = rect.localPosition;

				var minPosition = clampRect.min - rect.rect.min;
				var maxPosition = clampRect.max - rect.rect.max;

				clamp.x = Mathf.Clamp(rect.localPosition.x, minPosition.x, maxPosition.x);
				clamp.y = Mathf.Clamp(rect.localPosition.y, minPosition.y, maxPosition.y);

				rect.localPosition = clamp;

			}

			if (scaleMode > 0) {

				var t = scaleMode switch {
					ScaleMode.Distance => Vector3.Distance(Camera.main.transform.position, master.transform.position + trackingOffset),
					ScaleMode.CameraHeight => Camera.main.transform.position.y,
					ScaleMode.HeightDifference => Camera.main.transform.position.y - (master.transform.position.y + trackingOffset.y),
					ScaleMode.TileHeightDifference => Camera.main.transform.position.y - unit.tile.center.y,
					_ => throw new ArgumentOutOfRangeException("Invalid enum value.", nameof(scaleMode)),
				};
				var scale = scaleCurve.Evaluate(t);
				container.transform.localScale = new Vector3(scale, scale, scale);

			}

		}
	}

	protected override void OnShow() {
		base.OnShow();
		rect = container.GetComponent<RectTransform>();
		if (sendUnit) {
			ValueReceiver.SendValue(container, unit);
		}
	}

}
