using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private static TurnManager _instance;

    public List<GameObject> units = new List<GameObject>();
    GameObject nextUnit;
    public static TurnManager instance
    {
        get
        {
            if (_instance != null) return _instance;
            _instance = GameObject.FindObjectOfType<TurnManager>();
            if (_instance != null) return _instance;
            return _instance = new GameObject(nameof(TurnManager)).AddComponent<TurnManager>();
        }
    }

    private int _turn = 1;
    public static int turn => instance._turn;

    //public static void PassTurn()
    //{
    //    instance._turn++;
    //}

    private void Start()
    {
        ClearAndCreateNewTurnOrder();
        CheckTurnOrder();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            PassTurn();
        }
    }

    public void PassTurn()
    {
        //TODO: UnitController gives turn to unit
        if (units.Count > 0 && units.Count != 1)
        {
            units.Remove(nextUnit);
            nextUnit = units[0];
            print(nextUnit.GetComponent<UnitIdentifier>().unit.unitName + " is next in turn!");
        } else
        {
            instance._turn++;
            print("Next turn! Turn: " + turn);
            ClearAndCreateNewTurnOrder();
            CheckTurnOrder();
            print(nextUnit.GetComponent<UnitIdentifier>().unit.unitName + " is next in turn!");
        }
    }

    void ClearAndCreateNewTurnOrder()  //Clear any old list if one happens to exist, add all units in the Units parent GameObject to the list
    {
        if (units.Count > 0)
        {
            units.Clear();
        }
        var unitP = GameObject.Find("Units");
        var j = unitP.transform.childCount;
        for (int i = 0; i < j; i++)
        {
            units.Add(unitP.transform.GetChild(i).gameObject);
        }
    }

    public void CheckTurnOrder()  //Check all the unit speeds in the list and organize them appropriately
    {
        units.Sort(SortBySpeed);
        nextUnit = units[0];
        //print(nextUnit.GetComponent<UnitIdentifier>().unit.unitName + " is the first unit!"); //Purely for testing purposes
    }

    static int SortBySpeed(GameObject u1, GameObject u2)
    {
        return u2.GetComponent<UnitIdentifier>().unit.attributes.speed.CompareTo(u1.GetComponent<UnitIdentifier>().unit.attributes.speed);
    }
}


