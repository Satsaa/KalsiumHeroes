

using System;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(NoTargetAbilityData), menuName = "DataSources/" + nameof(NoTargetAbilityData))]
public class NoTargetAbilityData : AbilityData {

	public override Type createTypeConstraint => typeof(NoTargetAbility);

}
