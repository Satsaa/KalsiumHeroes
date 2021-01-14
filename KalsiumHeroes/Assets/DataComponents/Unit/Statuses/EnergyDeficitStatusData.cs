﻿
using System;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(EnergyDeficitStatusData), menuName = "DataSources/Statuses/" + nameof(EnergyDeficitStatusData))]
public class EnergyDeficitStatusData : StatusData {

	public override Type componentConstraint => typeof(EnergyDeficitStatus);

	[Header("Energy Deficit Status Data")]

	[Tooltip("Amount of defense reductions per stack.")]
	public int defenseReduction = 5;

	[Tooltip("Amount of resistance reductions per stack.")]
	public int resistanceReduction = 5;

}
