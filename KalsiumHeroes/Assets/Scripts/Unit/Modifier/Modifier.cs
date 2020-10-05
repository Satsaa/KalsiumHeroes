using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Muc.Systems.Values;

[CreateAssetMenu(fileName = "New Modifier", menuName = "KalsiumHeroes/Modifiers/Empty")]
public class Modifier : ScriptableObject {

  public string displayName;
  public DebuffType debuffType = DebuffType.None;
  public bool positive = true;
  public bool dispellable = true;
  public Attribute<int> turnDuration = new Attribute<int>(-1);

  [SerializeField] Unit unit;

  public virtual void OnAdd(Unit unit) { this.unit = unit; }
  public virtual void OnRemove() { }

  public virtual int OnGetSpeed(int value) => value;
  public virtual int OnGetMovement(int value) => value;
  public virtual int OnGetdefense(int value) => value;
  public virtual int OnGetResistance(int value) => value;

  public virtual float OnGetHealth(float value) => value;
  public virtual float OnHeal(float value) => value;
  public virtual float OnDamage(float value, DamageType type) => value;

  public virtual void OnModifierAdd(Modifier modifier) { }
  public virtual void OnModifierRemove(Modifier modifier) { }

  public virtual void OnUnitDispell() { }
  public virtual void OnPurge() { }

  public virtual void OnRoundStart() { }
  public virtual void OnRoundEnd() { }

  public virtual void OnTurnStart() { }
  public virtual void OnTurnEnd() { if (turnDuration.baseValue != -1 && --turnDuration.value <= 0) unit.RemoveModifier(this); }

  public virtual void OnKill() { }
  public virtual void OnSpawn() { }
}
