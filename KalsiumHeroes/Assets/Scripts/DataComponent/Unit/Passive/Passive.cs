﻿
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Passives are Modifiers that are displayed like Abilities.
/// </summary>
public abstract class Passive : UnitModifier {

	public PassiveData passiveData => (PassiveData)data;
	public override Type dataType => typeof(PassiveData);

}