using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatusTrifectaSilence", menuName = "KalsiumHeroes/Statuses/TrifectaSilence", order = 0)]
public class statusTrifectaSilence : StatusBase
{
    public StatusBase nextStatus;
    public override void OnStatusEnd(StatusManager smanager)
    {
        smanager.unitCanCast = true;
        smanager.AddStatusQueue(nextStatus);
    }

    public override void StatusRoundBegin(StatusManager smanager)
    {
        smanager.unitCanCast = false;
    }

    public override void StatusRoundEnd(StatusManager smanager)
    {
        return;
    }
}
