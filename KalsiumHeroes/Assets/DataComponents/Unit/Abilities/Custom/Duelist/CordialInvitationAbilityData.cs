using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(CordialInvitationAbilityData), menuName = "DataSources/Units/Duelist/" + nameof(CordialInvitationAbilityData))]
public class CordialInvitationAbilityData : AbilityData
{
    public override Type componentConstraint => typeof(CordialInvitationAbility);

    public CordialInvitationStatusData statusModifier;
}
