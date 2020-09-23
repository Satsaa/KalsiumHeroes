using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityBase : ScriptableObject
{
    [Tooltip("The name of the ability as shown in-game")]
    public string abilityName;

    [Tooltip("The description of the ability as shown in-game")]
    public string abilityDescription;

    [Tooltip("The type of the ability. Physical = Disabled by 'Disarm', applicable damage reduced by the 'Defense' stat. Spell = Disabled by 'Silence', applicable damage reduced by 'Resistance' stat.")]
    public AbilityClass abilityClass;

    [Tooltip("What, if any, statuses the ability inflicts")]
    public StatusBase abilityStatus;

    [Tooltip("How much damage does the ability do? If the ability isn't meant to do damage, leave this at 0.")]
    public float abilityBaseDamage;

    [Tooltip("How many turns it takes for an ability to be used again. 0 = No Cooldown")]
    public int abilityCooldown;
    [Tooltip("The radius of the ability. Cannot be lower than 1.")]
    public int abilityRadius = 1;
    [Tooltip("Whether or not the ability is ready on start. If false, it will be available after the cooldown")]
    public bool abilityReady = true;
    [Tooltip("How many limited uses does the ability have. 0 = Unlimited uses")]
    public int abilityUses;

    public Animation abilityAnimation;


    public abstract void TriggerAbility();
}
