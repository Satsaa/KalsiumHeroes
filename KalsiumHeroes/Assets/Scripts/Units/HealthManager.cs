using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    UnitBase unitBase;
    
    public float health;
    float maxHealth;
    public float defense;
    public float resistance;


    void Start()
    {
        unitBase = GetComponent<UnitIdentifier>().unit;
        health = unitBase.attributes.health;
        maxHealth = health;
        defense = unitBase.attributes.defense;
        resistance = unitBase.attributes.resistance;
    }


    public void SubtractHealth(float amount, AbilityClass type)
    {
        if (amount > 0)
        {
            if (type == AbilityClass.Weaponskill)
            {
                var damageReduction = (defense / 100) * amount;
                amount -= damageReduction;
                health -= amount;
            }
            if (type == AbilityClass.Spell)
            {
                var damageReduction = (resistance / 100) * amount;
                amount -= damageReduction;
                health -= amount;
            }
            if (type == AbilityClass.Skill)
            {
                health -= amount;
            }
        } else
        {
            health -= amount;
            if (health > maxHealth)
            {
                health = maxHealth;
            }
        }
    }
}
