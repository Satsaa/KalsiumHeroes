
using System;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(EdgeModifierData), menuName = "DataSources/" + nameof(EdgeModifierData))]
public class EdgeModifierData : ModifierData {

	public override Type createTypeConstraint => typeof(EdgeModifier);

}
