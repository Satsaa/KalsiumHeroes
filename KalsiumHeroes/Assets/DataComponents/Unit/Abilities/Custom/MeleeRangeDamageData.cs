
using System;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(MeleeRangeDamageData), menuName = "DataSources/" + nameof(MeleeRangeDamageData))]
public class MeleeRangeDamageData : AbilityData {

	public override Type componentTypeConstraint => typeof(MeleeRangeDamageAbility);

	[Header("Melee Range Damage Ability Data")]

	public Attribute<float> damage;

	public DamageType damageType;

}
