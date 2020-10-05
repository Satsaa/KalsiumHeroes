using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Unit : MonoBehaviour {


  [SerializeField] UnitAttributes attribs = default;
  [SerializeField] List<Modifier> modifiers = new List<Modifier>();
  [SerializeField] List<Ability> abilities = new List<Ability>();


  public int GetSpeed() => modifiers.Aggregate(attribs.speed.value, (cur, v) => v.OnGetSpeed(cur));
  public int GetMovement() => modifiers.Aggregate(attribs.movement.value, (cur, v) => v.OnGetMovement(cur));
  public int Getdefense() => modifiers.Aggregate(attribs.defense.value, (cur, v) => v.OnGetdefense(cur));
  public int GetResistance() => modifiers.Aggregate(attribs.resistance.value, (cur, v) => v.OnGetResistance(cur));
  public float GetHealth() => modifiers.Aggregate(attribs.health.value, (cur, v) => v.OnGetHealth(cur));

  public void Heal(float value) => attribs.health.value += modifiers.Aggregate(value, (cur, v) => v.OnHeal(cur));
  public void Damage(float value, DamageType type) {
    var total = modifiers.Aggregate(value, (cur, v) => v.OnDamage(cur, type));

    switch (type) {
      case DamageType.Physical:
        attribs.health.value -= Mathf.Max((1 - attribs.defense.value / 100f) * total);
        break;
      case DamageType.Magical:
        attribs.health.value -= Mathf.Max((1 - attribs.resistance.value / 100f) * total);
        break;
      case DamageType.Pure:
        attribs.health.value -= Mathf.Max(total);
        break;
      case DamageType.None:
      default:
        attribs.health.value -= Mathf.Max(total);
        Debug.LogWarning($"Damage type was either unknown or None. Damage was applied as {DamageType.Pure}");
        break;
    }
  }

  public bool AddModifier(Modifier modifier) {
    if (modifiers.Contains(modifier)) return false;
    foreach (var other in modifiers) other.OnModifierAdd(modifier);
    modifiers.Add(modifier);
    modifier.OnAdd(this);
    return true;
  }

  public bool RemoveModifier(Modifier modifier) {
    if (modifiers.Remove(modifier)) {
      modifier.OnRemove();
      foreach (var other in modifiers) other.OnModifierRemove(modifier);
      return true;
    }
    return false;
  }

  public void Dispell() {

  }
}