using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(RockSolidPassiveData), menuName = "DataSources/Units/Golem/" + nameof(RockSolidPassiveData))]
public class RockSolidPassiveData : PassiveData {

	public override Type createTypeConstraint => typeof(RockSolidPassive);

	[Tooltip("Matching units give resistances.")]
	public UnitTargetType filter;

	[Tooltip("Rings around the caster and their values")]
	public Ring[] rings;

	[Serializable]
	public class Ring {

		[Tooltip("Added Defense and Resistance from units in this ring.")]
		public Attribute<int> increase;
	}
}
