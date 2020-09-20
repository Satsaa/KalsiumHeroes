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

    void Update()
    {
        if (Input.GetKeyDown(poisonKey))
        {
            GetComponent<StatusManager>().AddStatus(poison.abilityStatus.status, poison.abilityStatus.duration);
            print("Test Unit was afflicted with " + poison.abilityName + " for " + poison.abilityStatus.duration + " turns!");
        }
        if (Input.GetKeyDown(healKey))
        {
            var oldHP = GetComponent<HealthManager>().health;
            GetComponent<HealthManager>().SubtractHealth(heal.abilityBaseDamage, heal.abilityClass);
            var newHP = GetComponent<HealthManager>().health;
            print("Test Unit was healed with ability " + heal.abilityName + " for " + (-heal.abilityBaseDamage) + ". Health: " + oldHP + " -> " + newHP);
        }
        if (Input.GetKeyDown(meleeKey))
        {
            var oldHP = GetComponent<HealthManager>().health;
            GetComponent<HealthManager>().SubtractHealth(melee.abilityBaseDamage, melee.abilityClass);
            var newHP = GetComponent<HealthManager>().health;
            print("Test Unit took damage from ability " + melee.abilityName + " for " + melee.abilityBaseDamage + " damage. Health: " + oldHP + " -> " + newHP);
        }
        if (Input.GetKeyDown(nextTurnKey))
        {
            TurnManager.PassTurn();
            print("It's a new turn! Turn: " + TurnManager.turn);
            var oldHP = GetComponent<HealthManager>().health;
            GetComponent<StatusManager>().CheckDisablesStart();
            GetComponent<StatusManager>().ActivateDamage();
            GetComponent<StatusManager>().ExpireActiveStatusEffects();
            var newHP = GetComponent<HealthManager>().health;
            if (oldHP > newHP)
            {
                print("Test Unit took damage from " + poison.abilityName + ". Health: " + oldHP + " -> " + newHP);
            }
        }
    }
}
