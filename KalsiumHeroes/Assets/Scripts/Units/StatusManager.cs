using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    //General booleans that certain statuses (any involving disarms, silences or roots) can use
    public bool unitCanCast = true;
    public bool unitCanAttack = true;
    public bool unitCanMove = true;

    UnitBase unit;

    public List<ActiveStatusEffect> activeStatus;
    public List<StatusBase> queue;

    //TODO Add Immunity Flags

    private void Start()
    {
        unit = GetComponent<UnitIdentifier>().unit;
    }

    public void AddStatus(StatusBase status)
    {
        var newStatus = new ActiveStatusEffect(TurnManager.turn, status);
        activeStatus.Add(newStatus);
    }

    public void AddStatusQueue(StatusBase status)
    {
        queue.Add(status);
        status.OnStatusStart(this);
    }

    public void CheckStatusStart()
    {
        if (queue.Count > 0)
        {
            foreach(StatusBase status in queue)
            {
                AddStatus(status);
            }
            queue.Clear();
        }
        if (activeStatus.Count > 0)
        {
            foreach (ActiveStatusEffect status in activeStatus)
            {
                status.status.StatusRoundBegin(this);
            }
        }
    }

    public void CheckStatusEnd()
    {
        if (activeStatus.Count > 0)
        {
            foreach (ActiveStatusEffect status in activeStatus)
            {
                status.status.StatusRoundEnd(this);
            }
            ExpireStatusEffects();
        }
    }

    public void ExpireStatusEffects()
    {
        if (activeStatus.Count > 0)
        {
            var expired = new List<ActiveStatusEffect>();
            foreach (ActiveStatusEffect status in activeStatus)
            {
                if (status.IsExpired())
                {
                    expired.Add(status);
                    status.status.OnStatusEnd(this);
                }
            }
            activeStatus = activeStatus.Where(s => !expired.Contains(s)).ToList<ActiveStatusEffect>();
        }
    }
}


[System.Serializable]
public class ActiveStatusEffect
{
    public int startingTurn;
    public StatusBase status;

    public ActiveStatusEffect(int startingTurn, StatusBase status)
    {
        this.startingTurn = startingTurn;
        this.status = status;
    }

    public bool IsExpired()
    {
        return startingTurn + status.duration <= TurnManager.turn;
    }
}
