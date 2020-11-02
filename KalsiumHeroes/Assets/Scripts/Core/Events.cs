
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public abstract class GameEvent {
  public int eventNum;
  public abstract void Handle();
}

[Serializable]
public class Events {

  [SerializeReference] public List<object> stack = new List<object>();
  public GameEvent first => (GameEvent)stack[0];
  public bool finished => stack.Count == 0 && (eHandler == null || eHandler.EventIsFinished());

  IEventHandler eHandler = null;

  public bool NextEvent() {
    if (stack.Count > 0 && (eHandler == null || eHandler.EventIsFinished())) {
      try {
        first.Handle();
      } catch (System.Exception e) {
        stack.RemoveAt(0);
        throw e;
      }
      stack.RemoveAt(0);
      return true;
    } else {
      return false;
    }
  }


  public static Dictionary<string, Type> events => _events ?? BuildEventTypes();
  private static Dictionary<string, Type> _events;

  private static Dictionary<string, Type> BuildEventTypes() {
    _events = new Dictionary<string, Type>();
    var nested = typeof(Events).GetNestedTypes().Where(t => t.BaseType == typeof(GameEvent));
    foreach (var type in nested) {
      _events.Add(type.Name, type);
    }
    return _events;
  }

  public void QueueEvent(GameEventPacket data) {
    var ge = (GameEvent)JsonUtility.FromJson(data.json, events[data.name]);
    stack.Add(ge);
  }


  // DO NOT CHANGE CLASS NAMES OF DEPLOYED GameEvents

  [Serializable]
  public class Ability : GameEvent {

    public string ability;
    public Vector3Int unit;
    public Vector3Int target;

    public override void Handle() {
      Debug.Log($"{this.GetType().Name}: Called");
      var unit = Game.grid.hexes[this.unit].unit;
      var ability = unit.abilities.FirstOrDefault(a => a.data.identifier == this.ability);
      if (ability == null) return;
      IEventHandler<Ability> asHandler = ability;
      Game.events.eHandler = asHandler;
      ability.OnCast();
      asHandler.StartEvent(this);
    }
  }

  [Serializable]
  public class Turn : GameEvent {

    public override void Handle() {
      Game.rounds.NextTurn();
      Debug.Log($"Next turn");
    }
  }

}