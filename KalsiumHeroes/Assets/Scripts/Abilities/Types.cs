using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The different types of abilities
public enum AbilityClass
{
    Weaponskill,
    Spell,
    Skill,
    Passive
}

//Flags for targeting, telling the game how each ability can be used
[System.Serializable]
public class TargetFlags
{
    public bool allies;
    public bool enemies;
    public bool ground;
    public bool self;

    [Tooltip("The range of the ability. 1 = Melee, 0 = Global")]
    public int abilityRange;
}

//The basic attributes each unit has
[System.Serializable]
public class UnitAttributes
{
    [Tooltip("The base health of the unit. Reach 0 and the unit dies")]
    public int health;
    [Tooltip("The amount of resistance to physical attacks the unit posesses. Read as percentages: 20 defense = 20% resistance to physical damage.")]
    public int defense;
    [Tooltip("The amount of resistance to magical attacks the unit posesses. Read as percentages: 20 resistance = 20% resistance to magical damage.")]
    public int resistance;
    [Tooltip("Speed determines when the unit gets to be played. The higher the speed, the higher the priority within any turn.")]
    public int speed;
    [Tooltip("Determines how many tiles any unit can move per turn.")]
    public int movement;
}

public enum ImmunityFlags
{
    None,
    Disarm,
    Root,
    Silence,
    Bleed,
    Poison,
    Plague
}

