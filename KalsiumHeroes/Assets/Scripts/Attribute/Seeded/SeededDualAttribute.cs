
#if UNITY_EDITOR
using UnityEditor;
using static Muc.Editor.EditorUtil;
using static Muc.Editor.PropertyUtil;
#endif

using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Serialization;
using System;
using System.Security;
using Muc;

[System.Serializable]
public class SeededDualAttribute<T> : SeededAttribute<T> {

  [SerializeField]
  [FormerlySerializedAs(nameof(other))]
  [Tooltip("Secondary value")]
  protected T _other;

  public virtual T other {
    get => otherAlterers.Values.Aggregate(_other, (current, alt) => alt(current));
  }

  protected Dictionary<object, Func<T, T>> otherAlterers = new Dictionary<object, Func<T, T>>();


  public SeededDualAttribute() { }
  public SeededDualAttribute(T value, T other) : base(value) {
    this._other = other;
  }

  /// <summary> Registers a function that alters what the other property returns. </summary>
  public void RegisterSecondaryAlterer(Func<T, T> alterer) {
    if (!AttributeBase.allow) throw new SecurityException("Configuring alterers is only allowed inside the RegisterAttributeAlterers function!");
    var keyObject = new object();
    otherAlterers.Add(keyObject, alterer);
    AttributeBase.keyTarget.Add(keyObject, this);
  }

  /// <summary> Internal use only. Attribute alterers are removed automatically. </summary>
  public override void RemoveAlterer(object key) {
    if (!AttributeBase.allow) throw new SecurityException("Configuring alterers is only allowed inside the RegisterAttributeAlterers function!");
    alterers.Remove(key);
    otherAlterers.Remove(key);
  }

}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SeededDualAttribute<>))]
public class SeededDualAttributeDrawer : PropertyDrawer {

  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

    using (PropertyScope(position, label, property, out label))
    using (RestoreLabelWidthScope())
    using (RestoreFieldWidthScope()) {

      var valueProperty = property.FindPropertyRelative("_value");
      var otherProperty = property.FindPropertyRelative("_other");

      var fieldInfo = GetFieldInfo(property);
      var labelAttribute = fieldInfo?.GetCustomAttributes(typeof(AttributeLabelsAttribute), false).FirstOrDefault() as AttributeLabelsAttribute;

      var noLabel = label.text is "" && label.image is null;
      var pos = position;
      pos.width = noLabel ? 0 : labelWidth;
      if (!noLabel) EditorGUI.LabelField(pos, label);

      using (IndentScope(v => 0)) {
        var valueLabel = new GUIContent(labelAttribute?.primaryLabel ?? "Seed");
        labelWidth = GUI.skin.label.CalcSize(valueLabel).x - spacing;
        pos.xMin = pos.xMax + spacing;
        pos.width = (position.xMax - pos.xMin) / 2 - spacing;
        using (DisabledScope(v => Application.isPlaying))
          EditorGUI.PropertyField(pos, valueProperty, valueLabel);

        var baseLabel = new GUIContent(labelAttribute?.secondaryLabel ?? "Other");
        labelWidth = GUI.skin.label.CalcSize(baseLabel).x - spacing;
        pos.xMin = pos.xMax + spacing;
        pos.xMax = position.xMax;
        using (DisabledScope(v => Application.isPlaying))
          EditorGUI.PropertyField(pos, otherProperty, baseLabel);
      }

    }

  }

}
#endif