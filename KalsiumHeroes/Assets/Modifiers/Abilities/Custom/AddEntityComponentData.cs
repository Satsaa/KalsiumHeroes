
using UnityEngine;

[CreateAssetMenu(fileName = nameof(AddEntityComponentData), menuName = "DataSources/" + nameof(AddEntityComponentData))]
public class AddEntityComponentData : AbilityData {

	[Header("Add Entity Component Ability Data")]

	[Tooltip("Added components. (Hint: Try adding a dot status effect!)")]
	public EntityComponentData[] components;

}
