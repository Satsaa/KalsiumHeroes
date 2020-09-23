using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusBase : ScriptableObject
{
    public string statusName;

    public string description;

    public int duration;

    public abstract void StatusRoundBegin(StatusManager smanager); //What the status does when the unit's turn begins (e.g. if the status is a silence, the unit cannot cast spells)

    public abstract void StatusRoundEnd(StatusManager smanager); //What the status does when the unit's turn ends (e.g. damage over time effects)

    public abstract void OnStatusEnd(StatusManager smanager); //What happens once the status has ended (e.g. begin a new status)
}
