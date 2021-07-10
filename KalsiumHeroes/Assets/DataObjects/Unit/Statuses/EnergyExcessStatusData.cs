
using System;
using Muc.Data;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(EnergyExcessStatusData), menuName = "DataSources/Statuses/" + nameof(EnergyExcessStatusData))]
public class EnergyExcessStatusData : StatusData {

	public override Type createTypeConstraint => typeof(EnergyExcessStatus);

	[Tooltip("Linearly stacking outgoing damage multiplications by DamageType.")]
	public SerializedDictionary<DamageType, float> dmgTypeMults;

	[Tooltip("Linearly stacking outgoing damage multiplications by AbilityType.")]
	public SerializedDictionary<AbilityType, float> abiTypeMults;

}
