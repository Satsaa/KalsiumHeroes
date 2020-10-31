#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Muc.Editor;

[RequireComponent(typeof(Unit))]
public class Modifier : EntityComponent {

  public ModifierData ModifierData => (ModifierData)data;
  public override Type dataType => typeof(ModifierData);

  [HideInInspector] public Unit unit;


  protected void OnValidate() {
    if (source) data = Instantiate(source);
    if (!unit) unit = GetComponent<Unit>();
  }

  protected void Awake() {
    data = Instantiate(source);
    unit = GetComponent<Unit>();
    Game.modifiers.RegisterModifier(this);
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

  /// <summary> When this Modifier is being added (instantiated). </summary>
  public virtual void OnAdd() { }
  /// <summary> When this Modifier is being removed (destroyed). </summary>
  public virtual void OnRemove() { }

  /// <summary> When any other Modifier is being added. </summary>
  public virtual void OnAdd(Modifier modifier) { }
  /// <summary> When any other Modifier is being removed. </summary>
  public virtual void OnRemove(Modifier modifier) { }

  public virtual float OnHeal(float value) => value;
  public virtual float OnDamage(float value, DamageType type) => value;

  /// <summary> When a round starts. </summary>
  public virtual void OnRoundStart() { }
  /// <summary> When the Unit's turn starts. </summary>
  public virtual void OnTurnStart() { }
  /// <summary> When the Unit's turn ends. </summary>
  public virtual void OnTurnEnd() { }

  /// <summary> When the Unit dies. </summary>
  public virtual void OnDeath() { }

  /// <summary> When the Unit casts an Ability that is not a base Ability. </summary>
  public virtual void OnAbilityCast(Ability ability) { }

  /// <summary> When the Unit casts a base Ability. </summary>
  public virtual void OnBaseAbilityCast(Ability ability) { }
}


#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(Modifier), true)]
public class ModifierEditor : Editor {

  SerializedProperty source;
  SerializedProperty data;

  Modifier t => (Modifier)target;

  void OnEnable() {
    source = serializedObject.FindProperty(nameof(Modifier.source));
    data = serializedObject.FindProperty(nameof(Modifier.data));
  }

  public override void OnInspectorGUI() {
    serializedObject.Update();

    using (EditorUtil.DisabledScope(v => Application.isPlaying))
      EditorGUILayout.ObjectField(source, t.dataType);

    using (EditorUtil.DisabledScope(v => !Application.isPlaying))
      EditorGUILayout.PropertyField(data);

    DrawPropertiesExcluding(serializedObject, nameof(Modifier.source), nameof(Modifier.data), "m_Script");

    serializedObject.ApplyModifiedProperties();
  }
}
#endif