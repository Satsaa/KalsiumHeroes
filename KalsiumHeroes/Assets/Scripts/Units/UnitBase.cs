using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitBase : ScriptableObject
{
    public string unitName;

    [Tooltip("The base attributes of any given unit.")]
    public UnitAttributes attributes;

    [Tooltip("Select which, if any, immunity to debuffs the unit has.")]
    public UnitImmunity immunities;

    [Tooltip("Every ability at the units disposal.")]
    public AbilityBase[] abilities;
}
