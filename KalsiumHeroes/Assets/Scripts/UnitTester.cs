using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTester : MonoBehaviour
{
    public AbilityBase poison;
    public KeyCode poisonKey;

    public AbilityBase heal;
    public KeyCode healKey;

    public AbilityBase melee;
    public KeyCode meleeKey;

    public KeyCode nextTurnKey;

    StatusManager sM;
    HealthManager hM;

    private void Start()
    {
        sM = GetComponent<StatusManager>();
        hM = GetComponent<HealthManager>();
        print("Test start! Turn: " + TurnManager.turn);
    }

    void Update()
    {
        if (Input.GetKeyDown(poisonKey))
        {
            sM.AddStatusQueue(poison.abilityStatus);
            print("Turn: " + TurnManager.turn + ": Test Unit was afflicted with " + poison.abilityName + " for " + poison.abilityStatus.duration + " turns!");
        }
        if (Input.GetKeyDown(healKey))
        {
            var oldHP = hM.health;
            hM.SubtractHealth(heal.abilityBaseDamage, heal.abilityClass);
            var newHP = hM.health;
            print("Turn: " + TurnManager.turn + ": Test Unit was healed with ability " + heal.abilityName + " for " + (-heal.abilityBaseDamage) + ". Health: " + oldHP + " -> " + newHP);
        }
        if (Input.GetKeyDown(meleeKey))
        {
            var oldHP = hM.health;
            hM.SubtractHealth(melee.abilityBaseDamage, melee.abilityClass);
            var newHP = hM.health;
            print("Turn: " + TurnManager.turn + ": Test Unit took damage from ability " + melee.abilityName + " for " + melee.abilityBaseDamage + " damage. Health: " + oldHP + " -> " + newHP);
        }
        if (Input.GetKeyDown(nextTurnKey))
        {
            var oldHP = hM.health;
            TurnManager.PassTurn();
            sM.CheckStatusEnd();
            var newHP = hM.health;
            if (oldHP > newHP)
            {
                print("Test Unit took damage from a status! Health: " + oldHP + " -> " + newHP);
            }
            print("It's the next turn! Turn: " + TurnManager.turn);
            sM.CheckStatusStart();
            if (!sM.unitCanAttack)
            {
                print("Test Unit is Disarmed! It can't use weaponskills this turn!");
            }
            if (!sM.unitCanMove)
            {
                print("Test Unit is Rooted! It can't move this turn!");
            }
            if (!sM.unitCanCast)
            {
                print("Test Unit is Silenced! It can't cast spells this turn!");
            }
        }
    }
}
