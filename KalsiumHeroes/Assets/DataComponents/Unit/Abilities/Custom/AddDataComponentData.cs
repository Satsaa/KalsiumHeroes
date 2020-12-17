
using UnityEngine;

[CreateAssetMenu(fileName = nameof(AddDataComponentData), menuName = "DataSources/" + nameof(AddDataComponentData))]
public class AddDataComponentData : AbilityData {

	[Header("Add Data Component Ability Data")]

	[Tooltip("Added components. (Hint: Try adding a dot status effect!)")]
	public DataComponentData[] components;

}
