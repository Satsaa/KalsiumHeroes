using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatusTrifectaStart", menuName = "KalsiumHeroes/Statuses/TrifectaStart", order = 0)]
public class statusTrifecta : StatusBase
{
    public StatusBase nextStatus;

    public override void OnStatusStart(StatusManager smanager)
    {
        throw new System.NotImplementedException();
    }
    public override void OnStatusEnd(StatusManager smanager)
    {
        smanager.unitCanMove = true;
        smanager.AddStatusQueue(nextStatus);
    }

    public override void StatusRoundBegin(StatusManager smanager)
    {
        smanager.unitCanMove = false;
    }

    public override void StatusRoundEnd(StatusManager smanager)
    {
        return;
    }
}
