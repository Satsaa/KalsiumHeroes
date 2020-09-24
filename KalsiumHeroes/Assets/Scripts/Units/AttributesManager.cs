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

    public void ChangeSpeed(int value)
    {
        speed += value;
    }

    public void SetSpeed(int value)
    {
        speed = value;
    }

    public void ResetSpeed(int value)
    {
        speed = baseSpeed;
    }

    public void ChangeMovement(int value)
    {
        movement += value;
    }

    public void SetMovement(int value)
    {
        movement = value;
    }

    public void ResetMovement(int value)
    {
        movement = baseMovement;
    }

    public void ChangeDefense(int value)
    {
        defense += value;
    }

    public void SetDefense(int value)
    {
        defense = value;
    }

    public void ResetDefense(int value)
    {
        defense = baseDefense;
    }

    public void ChangeResistance(int value)
    {
        resistance += value;
    }

    public void SetResistance(int value)
    {
        resistance = value;
    }

    public void ResetResistance(int value)
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
