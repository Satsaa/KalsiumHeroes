
using Muc.Editor;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(UnitModifierData), menuName = "DataSources/" + nameof(UnitModifierData))]
public class UnitModifierData : ModifierData {

	[Header("Unit Modifier Data")]
	[Tooltip("Displayed image")]
	public Sprite sprite;

}
