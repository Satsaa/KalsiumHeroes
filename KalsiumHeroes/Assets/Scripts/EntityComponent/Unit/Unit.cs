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

  [field: SerializeField] public List<Modifier> modifiers { get; private set; } = new List<Modifier>();
  [field: SerializeField] public List<Ability> abilities { get; private set; } = new List<Ability>();
  [field: SerializeField] public List<Passive> passives { get; private set; } = new List<Passive>();
  [field: SerializeField] public List<StatusEffect> statuses { get; private set; } = new List<StatusEffect>();


  [HideInInspector]
  [Tooltip("Unit is silenced? It cannot cast spells.")]
  public SeededAttribute<bool> silenced;

  [HideInInspector]
  [Tooltip("Unit is disarmed? It cannot cast weapon skills.")]
  public SeededAttribute<bool> disarmed;

  [HideInInspector]
  [Tooltip("Unit is rooted? It cannot move.")]
  public SeededAttribute<bool> rooted;

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

  public void RegisterModifier(Modifier modifier) {
    modifiers.Add(modifier);
    if (modifier is Ability ability) abilities.Add(ability);
    if (modifier is Passive passive) passives.Add(passive);
    if (modifier is StatusEffect status) statuses.Add(status);
  }

  void ClampHealth() {
    if (unitData.health.value < 0) {
      unitData.health.value = 0;
      return;
    }
    unitData.health.LimitValue();
  }

  public void Heal(float value) {
    unitData.health.value += modifiers.Aggregate(Max(0, value), (cur, v) => Max(0, v.OnHeal(cur)));
    ClampHealth();
  }

  public void Damage(float value, DamageType type) {
    var total = modifiers.Aggregate(Max(0, value), (cur, v) => Max(0, v.OnDamage(cur, type)));

    switch (type) {
      case DamageType.Physical:
        unitData.health.value -= (1 - unitData.defense.value / 100f) * total;
        ClampHealth();
        break;
      case DamageType.Magical:
        unitData.health.value -= (1 - unitData.resistance.value / 100f) * total;
        ClampHealth();
        break;
      case DamageType.Pure:
        unitData.health.value -= total;
        ClampHealth();
        break;
      case DamageType.None:
      default:
        unitData.health.value -= total;
        ClampHealth();
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

  public void Dispell() {
    foreach (var effect in statuses) {
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
