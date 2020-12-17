
using Muc.Editor;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(EdgeModifierData), menuName = "DataSources/" + nameof(EdgeModifierData))]
public class EdgeModifierData : DataComponentData {

	[Header("Edge Modifier Data")]
	[Tooltip("Placeholder for properties of EdgeModifiers")]
	public int tilePlaceholder;

}
