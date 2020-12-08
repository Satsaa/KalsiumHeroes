
using Muc.Editor;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(ModifierData), menuName = "DataSources/" + nameof(ModifierData))]
public class ModifierData : EntityComponentData {

	[Header("Unit Modifier Data")]
	[Tooltip("Displayed image")]
	public Sprite sprite;

}
