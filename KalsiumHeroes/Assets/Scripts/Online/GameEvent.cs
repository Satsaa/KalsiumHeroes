

using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
/// <summary>
/// </summary>
public class GameEvent<T> {

  public string code = typeof(T).Name;
  public int eventNum;

  public event Action<T> OnEvent;

  public virtual void Invoke(string json) {
    var e = JsonUtility.FromJson<T>(json);
    OnEvent.Invoke(e);
  }

}

public static class GameEvents {

  public static Dictionary<string, Type> types => _types ?? BuildEventTypes();
  private static Dictionary<string, Type> _types;

  private static Dictionary<string, Type> BuildEventTypes() {
    _types = new Dictionary<string, Type>();
    var nested = typeof(GameEvents).GetNestedTypes();
    foreach (var type in nested) {
      _types.Add(type.Name, type);
    }
    return _types;
  }

  // DO NOT CHANGE CLASS NAMES OF DEPLOYED GameEvents
  // ONLY ADD CLASSES INHERITING GameEvents
  // GameEvents APPLY TO UNITS UNLESS SPECIFIED OTHERWISE

  [System.Serializable]
  public class Position : GameEvent<Position> {
    public Vector3 unit;
    public Vector3 target;
  }

  [System.Serializable]
  public class Ability : GameEvent<Ability> {
    public string ability;
    public Vector3 unit;
    public Vector3 target;
  }

  [System.Serializable]
  public class Health : GameEvent<Health> {
    public Vector3 unit;
    public float health;
  }

  [System.Serializable]
  public class Damage : GameEvent<Damage> {
    public Vector3 unit;
    public float health;
  }

  [System.Serializable]
  public class Heal : GameEvent<Heal> {
    public Vector3 unit;
    public float health;
  }

  [System.Serializable]
  public class Unit : GameEvent<Unit> {
    public string unit;
    public Vector3 target;
  }

  [System.Serializable]
  public class Remove : GameEvent<Remove> {
    public Vector3 target;
  }
}