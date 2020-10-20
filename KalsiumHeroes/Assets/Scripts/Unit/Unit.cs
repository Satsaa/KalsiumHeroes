using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Mathf;

public class Unit : MonoBehaviour, IEventHandler<Events.Move> {

  [SerializeField] protected UnitData source;
  [SerializeField, HideInInspector] protected UnitData data;

  public UnitModifier[] modifiers => GetComponents<UnitModifier>();
  public Ability[] abilities => GetComponents<Ability>();
  public StatusEffect[] effects => GetComponents<StatusEffect>();

  public Team team;
  public GameHex hex;

  protected void OnValidate() {
    if (source) data = Instantiate(source);
    if (hex && hex.unit == null) {
      MovePosition(hex);
    }
  }

  protected void Awake() {
    data = Instantiate(source);
    if (hex && (hex.unit == this || hex.unit == null)) {
      MovePosition(hex);
    } else {
      // Move unit to first unoccupied hex
      foreach (var hex in Game.grid.hexes.Values) {
        if (hex.unit == null && !hex.blocked) {
          MovePosition(hex);
          break;
        }
      }
    }
  }

  public int GetSpeed() => modifiers.Aggregate(data.speed.value, (cur, v) => v.OnGetSpeed(cur));
  public int GetMovement() => modifiers.Aggregate(data.movement.value, (cur, v) => v.OnGetMovement(cur));
  public int Getdefense() => modifiers.Aggregate(data.defense.value, (cur, v) => v.OnGetDefense(cur));
  public int GetResistance() => modifiers.Aggregate(data.resistance.value, (cur, v) => v.OnGetResistance(cur));
  public float GetHealth() => modifiers.Aggregate(data.health.value, (cur, v) => v.OnGetHealth(cur));
  public float GetMaxHealth() => modifiers.Aggregate(data.maxHealth.value, (cur, v) => v.OnGetMaxHealth(cur));

  public void Heal(float value) {
    data.health.value += modifiers.Aggregate(Max(0, value), (cur, v) => Max(0, v.OnHeal(cur)));
    MaxHealth();
  }
  public void Damage(float value, DamageType type) {
    var total = modifiers.Aggregate(Max(0, value), (cur, v) => Max(0, v.OnDamage(cur, type)));

    switch (type) {
      case DamageType.Physical:
        data.health.value -= (1 - data.defense.value / 100f) * total;
        MaxHealth();
        break;
      case DamageType.Magical:
        data.health.value -= (1 - data.resistance.value / 100f) * total;
        MaxHealth();
        break;
      case DamageType.Pure:
        data.health.value -= total;
        MaxHealth();
        break;
      case DamageType.None:
      default:
        data.health.value -= total;
        MaxHealth();
        Debug.LogWarning($"Damage type was either unknown or None. Damage was applied as {DamageType.Pure}");
        break;
    }
  }

  void MaxHealth() => data.health.value = Mathf.Min(data.health.value, data.maxHealth.value);

  public void Dispell() {
    foreach (var effect in effects) {
      effect.OnDispell();
    }
  }

  public bool MovePosition(GameHex hex, bool reposition = true) {
    if (hex.unit == null) {
      this.hex.unit = null;
      hex.unit = this;
      this.hex = hex;
      if (reposition) transform.position = hex.center;
      return true;
    } else {
      return hex.unit == this;
    }
  }

  public void SwapPosition(GameHex hex, bool reposition = true) {
    if (hex.unit == null) {
      this.hex.unit = null;
      hex.unit = this;
      this.hex = hex;
      transform.position = hex.center;
    } else {
      var other = hex.unit;
      var sourceHex = this.hex;

      hex.unit = this;
      this.hex = hex;

      sourceHex.unit = other;
      other.hex = sourceHex;

      // Only this units movement is optional
      if (reposition) transform.position = hex.center;
      other.transform.position = sourceHex.center;
    }
  }

  void IEventHandler<Events.Move>.StartEvent(Events.Move data) {
    var source = this.hex;
    var target = Game.grid.hexes[data.target];
    SwapPosition(target);
  }

  bool IEventHandler.EventIsFinished() {
    return true;
  }

  bool IEventHandler.SkipEvent() {
    return true;
  }
}

// Classes
public class A : MonoBehaviour {
  public string displayName;
  public int duration;
}

public class B : A {
  public int damage;
}

// Essentially results in
public class C {
  public string displayName;
  public int duration;
  public int damage;
}



// Data containers
public class XData : ScriptableObject {
  public string displayName;
  public int duration;
}

public class YData : ScriptableObject {
  public int damage;
}

// Classes
public class X : MonoBehaviour {
  public XData xData;
}

public class Y : X {
  public YData yData;
}

// Essentially results in
public class Z {
  public XData xData;
  public YData yData;
}