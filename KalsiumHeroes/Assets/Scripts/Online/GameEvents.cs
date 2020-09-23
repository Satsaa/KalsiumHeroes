

using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using Object = UnityEngine.Object;

[Serializable]
public abstract class GameEvent {
  public int eventNum;
}

public class GameEvents : ScriptableObject {

  public static Dictionary<string, Type> events => _events ?? BuildEventTypes();
  private static Dictionary<string, Type> _events;

  private static Dictionary<string, Type> BuildEventTypes() {
    _events = new Dictionary<string, Type>();
    var nested = typeof(GameEvents).GetNestedTypes().Where(t => t.BaseType == typeof(GameEvent));
    foreach (var type in nested) _events.Add(type.Name, type);
    return _events;
  }

  public void InvokeEvent(GameEventPacket data) {
    switch (data.name) {
      case nameof(Position): break;
      case nameof(Ability): break;
      case nameof(Health): break;
      case nameof(Damage): break;
      case nameof(Heal): break;
      case nameof(Create): break;
      case nameof(Remove): break;
      default: Debug.LogError($"Unknown {nameof(GameEvent)} name {data.name}"); break;
    }
  }


  // DO NOT CHANGE CLASS NAMES OF DEPLOYED GameEvents
  // ONLY ADD CLASSES INHERITING GameEvent
  // GameEvents APPLY TO UNITS UNLESS SPECIFIED OTHERWISE


  [Serializable]
  public class Position : GameEvent {
    public Vector3Int unit;
    public Vector3Int target;
  }

  [Serializable]
  public class Ability : GameEvent {
    public string ability;
    public Vector3Int unit;
    public Vector3Int target;
  }

  [Serializable]
  public class Health : GameEvent {
    public Vector3Int unit;
    public float health;
  }

  [Serializable]
  public class Damage : GameEvent {
    public Vector3Int unit;
    public float health;
  }

  [Serializable]
  public class Heal : GameEvent {
    public Vector3Int unit;
    public float health;
  }

  [Serializable]
  public class Create : GameEvent {
    public string unit;
    public Vector3Int target;
  }

  [Serializable]
  public class Remove : GameEvent {
    public Vector3Int target;
  }
}