
using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = nameof(AddModifierAbilityData), menuName = "DataSources/Abilities/" + nameof(AddModifierAbilityData))]
public class AddModifierAbilityData : AbilityData {

	public override Type createTypeConstraint => typeof(AddModifierAbility);

	[Tooltip("Added modifiers. (Hint: Try adding a dot status effect!)")]
	public ModifierData[] addedModifiers;

}
