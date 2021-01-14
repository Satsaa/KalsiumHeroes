
using System;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(AddDataComponentAbilityData), menuName = "DataSources/Abilities/" + nameof(AddDataComponentAbilityData))]
public class AddDataComponentAbilityData : AbilityData {

	public override Type componentConstraint => typeof(AddDataComponentAbility);

	[Header("Add Data Component Ability Data")]

	[Tooltip("Added components. (Hint: Try adding a dot status effect!)")]
	public DataComponentData[] components;

}
