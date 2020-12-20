using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(MoveCostEdgeModifierData), menuName = "DataSources/" + nameof(MoveCostEdgeModifierData))]
public class MoveCostEdgeModifierData : EdgeModifierData {

	[Header("MoveCost Edge Modifier Data")]
	[Tooltip("Aditional move cost.")]
	public Attribute<float> additionalMoveCost = new Attribute<float>(1);

}
