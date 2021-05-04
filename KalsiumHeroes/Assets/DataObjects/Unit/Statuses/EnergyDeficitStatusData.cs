
using System;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(EnergyDeficitStatusData), menuName = "DataSources/Statuses/" + nameof(EnergyDeficitStatusData))]
public class EnergyDeficitStatusData : StatusData {

	public override Type createTypeConstraint => typeof(EnergyDeficitStatus);

	[Tooltip("Amount of defense change per stack.")]
	public Attribute<int> defenseChange = new(-5);

	[Tooltip("Amount of resistance change per stack.")]
	public Attribute<int> resistanceChange = new(-5);

}
