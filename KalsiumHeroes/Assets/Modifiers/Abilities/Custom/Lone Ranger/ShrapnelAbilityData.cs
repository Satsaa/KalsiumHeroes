using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(ShrapnelAbilityData), menuName = "DataSources/Units/Lone Ranger/" + nameof(ShrapnelAbilityData))]
public class ShrapnelAbilityData : AbilityData {

	[Header("Shrapnel Ability Data")]
	public Attribute<float> damage;

	public DamageType damageType;

	[Tooltip("This modifier casts the shrapnel spell next turn. If you chose the correct one.")]
	public EntityComponentData shrapnelModifierData;

}
