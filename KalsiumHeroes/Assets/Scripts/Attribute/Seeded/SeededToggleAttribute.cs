﻿
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
public class SeededToggleAttribute<T> : SeededAttribute<T> {

  [Tooltip("Attribute is enabled?")]
  public bool enabled;

  protected Dictionary<object, Func<bool, bool>> enabledAlterers = new Dictionary<object, Func<bool, bool>>();


  public SeededToggleAttribute(bool enabled = true) {
    this.enabled = enabled;
  }
  public SeededToggleAttribute(T value, bool enabled = true) : base(value) {
    this.enabled = enabled;
  }


  /// <summary> Registers a function that alters what the other property returns. </summary>
  public void RegisterSecondaryAlterer(Func<bool, bool> alterer) {
    if (!AttributeBase.allow) throw new SecurityException("Configuring alterers is only allowed inside the RegisterAttributeAlterers function!");
    var keyObject = new object();
    enabledAlterers.Add(keyObject, alterer);
    AttributeBase.keyTarget.Add(keyObject, this);
  }

  /// <summary> Internal use only. Attribute alterers are removed automatically. </summary>
  public override void RemoveAlterer(object key) {
    if (!AttributeBase.allow) throw new SecurityException("Configuring alterers is only allowed inside the RegisterAttributeAlterers function!");
    alterers.Remove(key);
    enabledAlterers.Remove(key);
  }

}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SeededToggleAttribute<>))]
public class SeededToggleAttribute : PropertyDrawer {

  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

    using (PropertyScope(position, label, property, out label))
    using (RestoreLabelWidthScope())
    using (RestoreFieldWidthScope()) {

      var enabledProperty = property.FindPropertyRelative(nameof(ToggleAttribute<int>.enabled));
      var valueProperty = property.FindPropertyRelative("_value");

      var fieldInfo = GetFieldInfo(property);
      var labelAttribute = fieldInfo?.GetCustomAttributes(typeof(AttributeLabelsAttribute), false).FirstOrDefault() as AttributeLabelsAttribute;

      var noLabel = label.text is "" && label.image is null;
      var pos = position;
      pos.width = noLabel ? 0 : labelWidth;
      if (!noLabel) EditorGUI.LabelField(pos, label);

      using (IndentScope(v => 0)) {
        pos.xMin = pos.xMax + spacing;
        pos.width = 15 + spacing;
        EditorGUI.PropertyField(pos, enabledProperty, GUIContent.none);

        pos.xMin = pos.xMax + spacing;
        pos.xMax = position.xMax;
        using (DisabledScope(v => Application.isPlaying))
          EditorGUI.PropertyField(pos, valueProperty, new GUIContent(labelAttribute?.primaryLabel ?? ""));
      }

    }

  }

}
#endif