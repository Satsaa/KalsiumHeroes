using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityClass
{
    Weaponskill,
    Spell,
    Skill,
    Passive
}

public class TargetFlags
{
    public bool allies;
    public bool enemies;
    public bool ground;
    public bool self;

    [Tooltip("The range of the ability. 1 = Melee, 0 = Global")]
    public int abilityRange;
}

