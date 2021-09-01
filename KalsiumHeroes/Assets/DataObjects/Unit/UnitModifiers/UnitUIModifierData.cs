using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(UnitUIModifierData), menuName = "DataSources/UnitModifiers/" + nameof(UnitUIModifierData))]
public class UnitUIModifierData : UnitModifierData {

	public override Type createTypeConstraint => typeof(UnitUIModifier);

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

}
