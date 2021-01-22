using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(MarkOfCastigationStatusData), menuName = "DataSources/Units/Rogue Mage Hunter/" + nameof(MarkOfCastigationStatusData))]
public class MarkOfCastigationStatusData : StatusData {

	public override Type ownerConstraint => typeof(MarkOfCastigationStatus);

	public Attribute<float> damage;
	public DamageType damageType;
	public UnitModifierData silenceModifier;
	public UnitModifierData markOfPreyModifier;
}
