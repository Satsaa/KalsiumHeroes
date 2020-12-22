
using System;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(AddDataComponentData), menuName = "DataSources/" + nameof(AddDataComponentData))]
public class AddDataComponentData : AbilityData {

	public override Type componentTypeConstraint => typeof(AddDataComponentAbility);

	[Header("Add Data Component Ability Data")]

	[Tooltip("Added components. (Hint: Try adding a dot status effect!)")]
	public DataComponentData[] components;

}
