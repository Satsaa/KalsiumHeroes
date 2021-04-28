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

	[Tooltip("Clamp the container RectTransform inside the screen.")]
	public bool clampToScreen;

	[Tooltip("Clamp area. (0,0),(1,1) = whole screen, (0.5,0.5),(1,1) = quarter of the screen.")]
	public Vector2 clampMin = new Vector2(0, 0);
	[Tooltip("Clamp area. (0,0),(1,1) = whole screen, (0.5,0.5),(1,1) = quarter of the screen.")]
	public Vector2 clampMax = new Vector2(1, 1);

}
