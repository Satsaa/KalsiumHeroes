using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// This script is old and deprecated, only stored for now for possible archive reasons. Don't touch it, don't look at it, and most importantly DON'T use it. It sucks. It's fucking terrible.

public class StatusManagerOld : MonoBehaviour
{
    UnitBase unit;
    UnitImmunity immunities;

    public List<ActiveStatusEffectOld> activeEffects = new List<ActiveStatusEffectOld>();

    public bool bleeding;
    int bleedStack;

    public bool disarmed;

    public bool poisoned;
    int poisonStack;

    public bool rooted;

    public bool silenced;

    float poisonDamage = 5;
    float bleedDamage = 5;


    void Start()
    {
        unit = GetComponent<UnitIdentifier>().unit;
        immunities = unit.immunities;
        Debug.LogError("Old and deprecated version of StatusManager being used. Replace with the new one ASAP!");
    }

    public void AddStatus(UnitImmunity statusEffect, int duration)
    {
        statusEffect = CheckImmunities(statusEffect);
        if (!CheckStatusBooleans(statusEffect))
        {
            CheckForDamage(statusEffect);
            var newStatus = new UnitStatus(statusEffect, duration);
            var newActiveStatus = new ActiveStatusEffectOld(TurnManager.turn, newStatus);
            activeEffects.Add(newActiveStatus);
        }
    }

    public void ExpireActiveStatusEffectOlds()
    {
        var expired = new List<ActiveStatusEffectOld>();
        foreach (ActiveStatusEffectOld status in activeEffects)
        {
            if (status.IsExpired())
            {
                if (status.status.status.poison)
                {
                    poisonStack--;
                }
                if (status.status.status.bleed)
                {
                    bleedStack--;
                }
                expired.Add(status);
            }
        }

        activeEffects = activeEffects.Where(s => !expired.Contains(s)).ToList<ActiveStatusEffectOld>();
    }

    UnitImmunity CheckImmunities(UnitImmunity status)
    {
        if (immunities.bleed && status.bleed)
        {
            status.bleed = false;
        }
        if (immunities.disarm && status.disarm)
        {
            status.disarm = false;
        }
        if (immunities.poison && status.poison)
        {
            status.poison = false;
        }
        if (immunities.root && status.root)
        {
            status.root = false;
        }
        if (immunities.silence && status.silence)
        {
            status.silence = false;
        }
        return status;
    }

    bool CheckStatusBooleans(UnitImmunity status)
    {
        return (!status.bleed && !status.disarm && !status.poison && !status.root && !status.silence);
    }

    void CheckForDamage(UnitImmunity status)
    {
        if (status.poison)
        {
            poisonStack++;
        }
        if (status.bleed)
        {
            bleedStack++;
        }
    }

    public bool HasActiveStatus()
    {
        return activeEffects.Count > 0;
    }

    public void CheckDisablesStart()
    {
        var bS = 0;
        var dS = 0;
        var pS = 0;
        var rS = 0;
        var sS = 0;
        foreach (ActiveStatusEffectOld effect in activeEffects)
        {
            if (effect.status.status.bleed)
            {
                bleeding = true;
                bS++;
            }
            if (effect.status.status.disarm)
            {
                disarmed = true;
                dS++;
            }
            if (effect.status.status.poison)
            {
                poisoned = true;
                pS++;
            }
            if (effect.status.status.root)
            {
                rooted = true;
                rS++;
            }
            if (effect.status.status.silence)
            {
                silenced = true;
                sS++;
            }
        }
        if (bS == 0)
        {
            bleeding = false;
        }
        if (dS == 0)
        {
            disarmed = false;
        }
        if (pS == 0)
        {
            poisoned = false;
        }
        if (rS == 0)
        {
            rooted = false;
        }
        if (sS == 0)
        {
            silenced = false;
        }
    }

    public void ActivateDamage()
    {
        if (bleeding || poisoned)
        {
            GetComponent<HealthManager>().SubtractHealth((poisonDamage * poisonStack) + (bleedDamage * bleedStack), AbilityClass.Skill);
            //TODO Add secondary Poison and Bleed effects!
        }
    }

    public void ClearStatus()
    {
        bleeding = false;
        disarmed = false;
        poisoned = false;
        rooted = false;
        silenced = false;
    }
}

[System.Serializable]
public class ActiveStatusEffectOld
{
    public int startingTurn;
    public UnitStatus status;

    public ActiveStatusEffectOld(int startingTurn, UnitStatus status)
    {
        this.startingTurn = startingTurn;
        this.status = status;
    }

    public bool IsExpired()
    {
        return startingTurn + status.duration <= TurnManager.turn;
    }
}

//The different statuses each unit can //IMPORTANT: THIS SHIT IS OLD AND DEPRECATED, IT IS ONLY STORED FOR ARCHIVE REASONS (Old StatusManager can't work without them)
[System.Serializable]
public class UnitImmunity //IF YOU MODIFY THIS, REMEMBER TO MODIFY STATUSMANAGER CHECKIMMUNITY() AS WELL
{
    public bool root;
    public bool disarm;
    public bool silence;
    public bool bleed;
    public bool poison;
}

[System.Serializable]
public class UnitStatus
{
    public UnitImmunity status;
    public int duration;

    public UnitStatus(UnitImmunity status, int duration)
    {
        this.status = status;
        this.duration = duration;
    }
}
