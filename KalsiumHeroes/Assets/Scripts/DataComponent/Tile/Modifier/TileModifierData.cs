
using Muc.Editor;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(TileModifierData), menuName = "DataSources/" + nameof(TileModifierData))]
public class TileModifierData : DataComponentData {

	[Header("Tile Modifier Data")]
	[Tooltip("Displayed image")]
	public Sprite sprite;

}
