using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private static TurnManager _instance;
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

    public static void PassTurn()
    {
        instance._turn++;
    }
}
