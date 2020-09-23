using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatusTrifectaDisarm", menuName = "KalsiumHeroes/Statuses/TrifectaDisarm", order = 0)]
public class statusTrifectaDisarm : StatusBase
{
    public override void OnStatusEnd(StatusManager smanager)
    {
        smanager.unitCanAttack = true;
    }

    public override void StatusRoundBegin(StatusManager smanager)
    {
        smanager.unitCanAttack = false;
    }

    public override void StatusRoundEnd(StatusManager smanager)
    {
        return;
    }
}
