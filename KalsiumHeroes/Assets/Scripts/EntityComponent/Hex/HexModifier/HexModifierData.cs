
using Muc.Editor;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(HexModifierData), menuName = "DataSources/" + nameof(HexModifierData))]
public class HexModifierData : EntityComponentData {

	[Header("Hex Modifier Data")]
	[Tooltip("Displayed image")]
	public Sprite sprite;

}
