using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    UnitBase unitBase;
    AttributesManager aM;
    
    public float health;
    float maxHealth;


    void Start()
    {
        unitBase = GetComponent<UnitIdentifier>().unit;
        aM = GetComponent<AttributesManager>();
        health = unitBase.attributes.health;
        maxHealth = health;
    }


    public void SubtractHealth(float amount, AbilityClass type)
    {
        if (amount > 0)
        {
            if (type == AbilityClass.Weaponskill)
            {
                float damageReduction = (aM.defense / 100f) * amount;
                amount -= damageReduction;
                amount = Mathf.Clamp(amount, 0, 999);
                health -= amount;
            }
            if (type == AbilityClass.Spell)
            {
                var damageReduction = (aM.resistance / 100f) * amount;
                amount -= damageReduction;
                amount = Mathf.Clamp(amount, 0, 999);
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
