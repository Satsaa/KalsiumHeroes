
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Passives are Modifiers that are displayed like Abilities.
/// </summary>
public abstract class Passive : UnitModifier {

	new public PassiveData source => (PassiveData)_source;
	new public PassiveData data => (PassiveData)_data;
	public override Type dataType => typeof(PassiveData);

}