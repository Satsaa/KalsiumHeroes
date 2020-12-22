
using System;
using Muc.Editor;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(EdgeModifierData), menuName = "DataSources/" + nameof(EdgeModifierData))]
public class EdgeModifierData : ModifierData {

	public override Type componentTypeConstraint => typeof(EdgeModifier);

	// [Header("Edge Modifier Data")]

}
