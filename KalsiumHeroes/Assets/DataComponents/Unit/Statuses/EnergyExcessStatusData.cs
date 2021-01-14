
using System;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(EnergyExcessStatusData), menuName = "DataSources/Statuses/" + nameof(EnergyExcessStatusData))]
public class EnergyExcessStatusData : StatusData {

	public override Type componentConstraint => typeof(EnergyExcessStatus);

	[Header("Energy Excess Status Data")]

	[Tooltip("Linearly stacking outgoing damage amplifications by type.")]
	public Amps amps;

}
