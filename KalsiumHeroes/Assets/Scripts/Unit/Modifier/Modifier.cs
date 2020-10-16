using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public class UnitModifier : MonoBehaviour {

  [Tooltip("Name displayed to users")]
  public string displayName;
  [Tooltip("String identifier of this Ability")]
  public string identifier;
  [Tooltip("Description displayed to users")]
  public string description;

  public Unit unit;

  protected void Awake() {
    if (!TryGetComponent<Unit>(out unit)) {
      Debug.LogWarning($"You added a {nameof(UnitModifier)} to an object which does not have a {nameof(Unit)} component. {nameof(UnitModifier)}s are designed for {nameof(Unit)}s only.");
    }
    OnAdd();
    foreach (var other in unit.modifiers.Where(mod => mod != this)) {
      other.OnAdd(this);
    }
  }

  protected void OnDestroy() {
    OnRemove();
    foreach (var other in unit.modifiers.Where(mod => mod != this)) {
      other.OnRemove(this);
    }
  }

  /// <summary> When this UnitModifier is being added. </summary>
  public virtual void OnAdd() { }
  /// <summary> When this UnitModifier is being removed. </summary>
  public virtual void OnRemove() { }

  /// <summary> When any other UnitModifier is being added. </summary>
  public virtual void OnAdd(UnitModifier modifier) { }
  /// <summary> When any other UnitModifier is being removed. </summary>
  public virtual void OnRemove(UnitModifier modifier) { }

  public virtual int OnGetSpeed(int value) => value;
  public virtual int OnGetMovement(int value) => value;
  public virtual int OnGetDefense(int value) => value;
  public virtual int OnGetResistance(int value) => value;
  public virtual float OnGetHealth(float value) => value;
  public virtual float OnGetMaxHealth(float value) => value;

  public virtual float OnHeal(float value) => value;
  public virtual float OnDamage(float value, DamageType type) => value;

  public virtual void OnRoundStart() { }
  public virtual void OnRoundEnd() { }

  /// <summary> When this Unit's turn starts. </summary>
  public virtual void OnTurnStart() { }
  /// <summary> When this Unit's turn ends. </summary>
  public virtual void OnTurnEnd() { }

  /// <summary> When this Unit dies. </summary>
  public virtual void OnKill() { }
  /// <summary> When this Unit spawns. </summary>
  public virtual void OnSpawn() { }

  /// <summary> When this Unit casts an Ability. </summary>
  public virtual void OnAbilityCast() { }
}
