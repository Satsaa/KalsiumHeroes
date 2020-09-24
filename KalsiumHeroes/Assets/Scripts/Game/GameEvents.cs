

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
  public abstract void InvokeLogic();
  public abstract void InvokeAnims();
}

[Serializable]
public class GameEvents {

  public static Dictionary<string, Type> events => _events ?? BuildEventTypes();
  private static Dictionary<string, Type> _events;

  private static Dictionary<string, Type> BuildEventTypes() {
    _events = new Dictionary<string, Type>();
    var nested = typeof(GameEvents).GetNestedTypes().Where(t => t.BaseType == typeof(GameEvent));
    foreach (var type in nested) _events.Add(type.Name, type);
    return _events;
  }

  public void InvokeEvent(GameEventPacket data) {
    var ge = (GameEvent)JsonUtility.FromJson(data.json, events[data.name]);
    ge.InvokeLogic();
    Game.anims.stack.Add(ge);
  }


  // DO NOT CHANGE CLASS NAMES OF DEPLOYED GameEvents
  // GameEvents APPLY TO UNITS UNLESS SPECIFIED OTHERWISE


  [Serializable]
  public class Move : GameEvent {
    public Vector3Int unit;
    public Vector3Int target;
    public override void InvokeLogic() => Game.logic.OnMove(this);
    public override void InvokeAnims() => Game.anims.AnimateMove(this);
  }

  [Serializable]
  public class Ability : GameEvent {
    public string ability;
    public Vector3Int unit;
    public Vector3Int target;
    public override void InvokeLogic() => Game.logic.OnAbility(this);
    public override void InvokeAnims() => Game.anims.AnimateAbility(this);
  }

  [Serializable]
  public class Health : GameEvent {
    public Vector3Int unit;
    public float health;
    public override void InvokeLogic() => Game.logic.OnHealth(this);
    public override void InvokeAnims() => Game.anims.AnimateHealth(this);
  }

  [Serializable]
  public class Damage : GameEvent {
    public Vector3Int unit;
    public float health;
    public override void InvokeLogic() => Game.logic.OnDamage(this);
    public override void InvokeAnims() => Game.anims.AnimateDamage(this);
  }

  [Serializable]
  public class Heal : GameEvent {
    public Vector3Int unit;
    public float health;
    public override void InvokeLogic() => Game.logic.OnHeal(this);
    public override void InvokeAnims() => Game.anims.AnimateHeal(this);
  }

  [Serializable]
  public class Create : GameEvent {
    public string unit;
    public Vector3Int target;
    public override void InvokeLogic() => Game.logic.OnCreate(this);
    public override void InvokeAnims() => Game.anims.AnimateCreate(this);
  }

  [Serializable]
  public class Remove : GameEvent {
    public Vector3Int target;
    public override void InvokeLogic() => Game.logic.OnRemove(this);
    public override void InvokeAnims() => Game.anims.AnimateRemove(this);
  }
}