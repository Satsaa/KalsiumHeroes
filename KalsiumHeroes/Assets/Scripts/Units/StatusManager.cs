using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    UnitBase unit;
    UnitImmunity immunities;

    public List<ActiveStatusEffect> activeEffects = new List<ActiveStatusEffect>();

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
    }

    public void AddStatus(UnitImmunity statusEffect, int duration)
    {
        statusEffect = CheckImmunities(statusEffect);
        if (!CheckStatusBooleans(statusEffect))
        {
            CheckForDamage(statusEffect);
            var newStatus = new UnitStatus(statusEffect, duration);
            var newActiveStatus = new ActiveStatusEffect(TurnManager.turn, newStatus);
            activeEffects.Add(newActiveStatus);
        }
    }

    public void ExpireActiveStatusEffects()
    {
        var expired = new List<ActiveStatusEffect>();
        foreach (ActiveStatusEffect status in activeEffects)
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

        activeEffects = activeEffects.Where(s => !expired.Contains(s)).ToList<ActiveStatusEffect>();
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
        foreach (ActiveStatusEffect effect in activeEffects)
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
public class ActiveStatusEffect
{
    public int startingTurn;
    public UnitStatus status;

    public ActiveStatusEffect(int startingTurn, UnitStatus status)
    {
        this.startingTurn = startingTurn;
        this.status = status;
    }

    public bool IsExpired()
    {
        return startingTurn + status.duration <= TurnManager.turn;
    }
}
