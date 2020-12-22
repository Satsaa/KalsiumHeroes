using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(AreaDamageSpellGenericData), menuName = "DataSources/" + nameof(AreaDamageSpellGenericData))]
public class AreaDamageSpellGenericData : AbilityData {

	public override Type componentTypeConstraint => typeof(AreaDamageSpellGenericAbility);

	[Header("Area Damage Spell Ability Data")]

	public Attribute<float> primaryDamage;

	public Attribute<float> secondaryDamage;

	public DamageType damageType;
}
