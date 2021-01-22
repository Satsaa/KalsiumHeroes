using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(AbilityUIModifierData), menuName = "DataSources/UnitModifiers/" + nameof(AbilityUIModifierData))]
public class AbilityUIModifierData : UnitModifierData {

	public override Type ownerConstraint => typeof(AbilityUIModifier);

	[Tooltip("World space offset or the UI")]
	public Vector3 wsOffset;
	[Tooltip("Screen space offset or the UI")]
	public Vector2 ssOffset;

	[Tooltip("A prefab with AbilityIcon component.")]
	public GameObject abilityPrefab;
	[Tooltip("A prefab with PassiveIcon component.")]
	public GameObject passivePrefab;

	[Space()]

	[Tooltip("Padding between icons.")]
	public float padding;

	[Range(0, 1)]
	[Tooltip("Alpha when not active.")]
	public float fadeAlpha = 0.15f;

	[Tooltip("Speed of alpha fade.")]
	public float alphaFadeSpeed = 5f;
}
