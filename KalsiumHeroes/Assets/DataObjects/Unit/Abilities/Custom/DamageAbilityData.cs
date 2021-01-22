
using System;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(DamageAbilityData), menuName = "DataSources/Abilities/" + nameof(DamageAbilityData))]
public class DamageAbilityData : AbilityData {

	public override Type createTypeConstraint => typeof(DamageAbility);

	public Attribute<float> damage;

	public DamageType damageType;

}
