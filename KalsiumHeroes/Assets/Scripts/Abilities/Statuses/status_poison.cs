﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatusPoison", menuName = "KalsiumHeroes/Statuses/Poison", order = 0)]
public class status_poison : StatusBase
{
    public float damageOverTime = 15;
    public int defReduction = -10;

    public override void OnStatusStart(StatusManager smanager)
    {
        smanager.GetComponent<AttributesManager>().AddDefense(defReduction);
    }
    public override void OnStatusEnd(StatusManager smanager)
    {
        smanager.GetComponent<AttributesManager>().AddDefense(-defReduction);
    }

    public override void StatusRoundBegin(StatusManager smanager)
    {
        return;
    }

    public override void StatusRoundEnd(StatusManager smanager)
    {
        HealthManager unitHM;
        if (smanager.GetComponent<HealthManager>() != null)
        {
            unitHM = smanager.GetComponent<HealthManager>();
            unitHM.SubtractHealth(damageOverTime, AbilityClass.Skill);
        } else Debug.LogError("No HealthManager found on unit!");
    }
}
