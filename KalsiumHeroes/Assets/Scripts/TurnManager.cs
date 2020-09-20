using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private static TurnManager _instance;
    public static TurnManager instance => _instance = _instance ?? GameObject.FindObjectOfType<TurnManager>() ?? new GameObject(nameof(TurnManager)).AddComponent<TurnManager>();

    private int _turn;
    public static int turn => instance._turn;

    public static void PassTurn()
    {
        instance._turn++;
    }
}
