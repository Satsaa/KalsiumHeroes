
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Passives are Modifiers that are displayed like Abilities.
/// </summary>
public abstract class Passive : UnitModifier {

	public new PassiveData source => (PassiveData)_source;
	public new PassiveData data => (PassiveData)_data;
	public override Type dataType => typeof(PassiveData);

}