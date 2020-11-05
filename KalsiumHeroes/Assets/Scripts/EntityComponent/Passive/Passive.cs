
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Passives are modifiers that are displayed like Abilities.
/// </summary>
public abstract class Passive : Modifier {

  public PassiveData passiveData => (PassiveData)data;
  public override Type dataType => typeof(PassiveData);

}