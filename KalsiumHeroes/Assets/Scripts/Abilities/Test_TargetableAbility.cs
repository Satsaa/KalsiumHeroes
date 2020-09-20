using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "KalsiumHeroes/Abilities/TestMelee", order = 0)]
public class Test_TargetableAbility : AbilityBase {

    [Tooltip("What can be targeted with the ability, and within what range.")]
    public TargetFlags targetConstraints;
    public override void TriggerAbility()
    {
        //TODO
    }
}
