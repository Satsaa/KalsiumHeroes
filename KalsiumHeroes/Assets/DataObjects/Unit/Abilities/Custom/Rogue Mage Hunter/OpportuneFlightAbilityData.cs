using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(OpportuneFlightAbilityData), menuName = "DataSources/Units/Rogue Mage Hunter/" + nameof(OpportuneFlightAbilityData))]
public class OpportuneFlightAbilityData : AbilityData {

	public override Type createTypeConstraint => typeof(OpportuneFlightAbility);

	public Attribute<int> moveDistance;
}
