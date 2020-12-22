
using System;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(DamageAbilityData), menuName = "DataSources/" + nameof(DamageAbilityData))]
public class DamageAbilityData : AbilityData {

	public override Type componentConstraint => typeof(DamageAbility);

	[Header("Damage Ability Data")]

	public Attribute<float> damage;

	public DamageType damageType;

}
