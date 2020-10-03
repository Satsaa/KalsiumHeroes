using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributesManager : MonoBehaviour
{
    UnitBase unit;

    public int speed;
    int baseSpeed;
    public int movement;
    int baseMovement;
    public int defense;
    int baseDefense;
    public int resistance;
    int baseResistance;
    void Start()
    {
        unit = GetComponent<UnitIdentifier>().unit;
        GetAttributes();
    }

    public void AddSpeed(int value)
    {
        speed += value;
    }

    public void SetSpeed(int value)
    {
        speed = value;
    }

    public void ResetSpeed()
    {
        speed = baseSpeed;
    }

    public void AddMovement(int value)
    {
        movement += value;
    }

    public void SetMovement(int value)
    {
        movement = value;
    }

    public void ResetMovement()
    {
        movement = baseMovement;
    }

    public void AddDefense(int value)
    {
        defense += value;
    }

    public void SetDefense(int value)
    {
        defense = value;
    }

    public void ResetDefense()
    {
        defense = baseDefense;
    }

    public void AddResistance(int value)
    {
        resistance += value;
    }

    public void SetResistance(int value)
    {
        resistance = value;
    }

    public void ResetResistance()
    {
        resistance = baseResistance;
    }

    public void ResetAttributes()
    {
        speed = baseSpeed;
        movement = baseMovement;
        defense = baseDefense;
        resistance = baseResistance;
    }

    void GetAttributes()
    {
        baseSpeed = unit.attributes.speed;
        baseMovement = unit.attributes.movement;
        baseDefense = unit.attributes.defense;
        baseResistance = unit.attributes.resistance;
        ResetAttributes();
    }
}
