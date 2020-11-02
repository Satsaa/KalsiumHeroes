using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Muc.Editor;
using static UnityEngine.Mathf;

public class Unit : EntityComponent {

  public UnitData unitData => (UnitData)data;
  public override Type dataType => typeof(UnitData);

  public Modifier[] modifiers => GetComponents<Modifier>();
  public Ability[] abilities => GetComponents<Ability>();
  public Passive[] passives => GetComponents<Passive>();
  public StatusEffect[] effects => GetComponents<StatusEffect>();


  [HideInInspector]
  [Tooltip("Unit is silenced? It cannot cast spells.")]
  public Attribute<bool> silenced;
  [HideInInspector]
  [Tooltip("Unit is silenced? It cannot cast weapon skills.")]
  public Attribute<bool> disarmed;

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

  public void Heal(float value) {
    unitData.health.value += modifiers.Aggregate(Max(0, value), (cur, v) => Max(0, v.OnHeal(cur)));
    LimitHealth();
  }

  public void Damage(float value, DamageType type) {
    var total = modifiers.Aggregate(Max(0, value), (cur, v) => Max(0, v.OnDamage(cur, type)));

    switch (type) {
      case DamageType.Physical:
        unitData.health.value -= (1 - unitData.defense.value / 100f) * total;
        LimitHealth();
        break;
      case DamageType.Magical:
        unitData.health.value -= (1 - unitData.resistance.value / 100f) * total;
        LimitHealth();
        break;
      case DamageType.Pure:
        unitData.health.value -= total;
        LimitHealth();
        break;
      case DamageType.None:
      default:
        unitData.health.value -= total;
        LimitHealth();
        Debug.LogWarning($"Damage type was either unknown or None. Damage was applied as {DamageType.Pure}");
        break;
    }

    if (unitData.health.value <= 0) {
      foreach (var modifier in Game.modifiers.GetModifiers()) {
        modifier.OnDeath();
      }
      // Health still deadly?
      if (unitData.health.value <= 0) {
        hex.graveYard.Add(new GraveUnit(this));
        Destroy(gameObject);
      }
    }
  }

  void LimitHealth() => unitData.health.value = Mathf.Min(unitData.health.value, unitData.health.other);

  public void Dispell() {
    foreach (var effect in effects) {
      effect.OnDispell();
    }
  }

  public bool MovePosition(GameHex hex, bool reposition = true) {
    if (hex.unit == null) {
      if (this.hex) this.hex.unit = null;
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

}
