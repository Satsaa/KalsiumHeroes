using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Mathf;

public class Unit : MonoBehaviour, IEventHandler<Events.Move> {

  [Tooltip("Speed determines when the unit gets to be played. The higher the speed, the higher the priority within any turn."), SerializeField]
  protected Attribute<int> speed = new Attribute<int>(1);

  [Tooltip("Determines how many tiles any unit can move per turn."), SerializeField]
  protected Attribute<int> movement = new Attribute<int>(1);

  [Tooltip("The health of the unit. Reach 0 and the unit dies."), SerializeField]
  protected Attribute<float> health = new Attribute<float>(100);

  [Tooltip("The maximum health of the unit."), SerializeField]
  protected Attribute<float> maxHealth = new Attribute<float>(100);

  [Tooltip("The amount of resistance to physical damage the unit posesses."), SerializeField]
  protected Attribute<int> defense;

  [Tooltip("The amount of resistance to magical damage the unit posesses."), SerializeField]
  protected Attribute<int> resistance;

  public Team team;
  public GameHex hex;

  public UnitModifier[] modifiers => GetComponents<UnitModifier>();
  public Ability[] abilities => GetComponents<Ability>();
  public StatusEffect[] effects => GetComponents<StatusEffect>();


  protected void OnValidate() {
    if (hex && hex.unit == null) {
      hex.unit = this;
      transform.position = hex.center;
    }
  }

  protected void Awake() {
    if (hex && (hex.unit == this || hex.unit == null)) {
      hex.unit = this;
      transform.position = hex.center;
    } else {
      // Move unit to first unoccupied hex
      foreach (var hex in Game.grid.hexes.Values) {
        if (hex.unit == null && !hex.blocked) {
          hex.unit = this;
          transform.position = hex.center;
          break;
        }
      }
    }
  }

  public int GetSpeed() => modifiers.Aggregate(speed.value, (cur, v) => v.OnGetSpeed(cur));
  public int GetMovement() => modifiers.Aggregate(movement.value, (cur, v) => v.OnGetMovement(cur));
  public int Getdefense() => modifiers.Aggregate(defense.value, (cur, v) => v.OnGetDefense(cur));
  public int GetResistance() => modifiers.Aggregate(resistance.value, (cur, v) => v.OnGetResistance(cur));
  public float GetHealth() => modifiers.Aggregate(health.value, (cur, v) => v.OnGetHealth(cur));
  public float GetMaxHealth() => modifiers.Aggregate(maxHealth.value, (cur, v) => v.OnGetMaxHealth(cur));

  public void Heal(float value) {
    health.value += modifiers.Aggregate(Max(0, value), (cur, v) => Max(0, v.OnHeal(cur)));
    MaxHealth();
  }
  public void Damage(float value, DamageType type) {
    var total = modifiers.Aggregate(Max(0, value), (cur, v) => Max(0, v.OnDamage(cur, type)));

    switch (type) {
      case DamageType.Physical:
        health.value -= (1 - defense.value / 100f) * total;
        MaxHealth();
        break;
      case DamageType.Magical:
        health.value -= (1 - resistance.value / 100f) * total;
        MaxHealth();
        break;
      case DamageType.Pure:
        health.value -= total;
        MaxHealth();
        break;
      case DamageType.None:
      default:
        health.value -= total;
        MaxHealth();
        Debug.LogWarning($"Damage type was either unknown or None. Damage was applied as {DamageType.Pure}");
        break;
    }
  }

  void MaxHealth() => health.value = Mathf.Min(health.value, maxHealth.value);

  public void Dispell() {
    foreach (var effect in effects) {
      effect.OnDispell();
    }
  }

  void IEventHandler<Events.Move>.StartEvent(Events.Move data) {
    var source = this.hex;
    var target = Game.grid.hexes[data.target];

    source.unit = target.unit;
    target.unit = this;
    if (target.unit != null) target.unit.transform.position = source.center;
    transform.position = target.center;
  }

  bool IEventHandler.EventIsFinished() {
    return true;
  }

  bool IEventHandler.SkipEvent() {
    return true;
  }
}