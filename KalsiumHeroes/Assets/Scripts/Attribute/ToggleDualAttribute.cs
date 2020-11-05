
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
public class ToggleDualAttribute<T> : DualAttribute<T> {

  [Tooltip("Attribute is enabled?")]
  public bool enabled;

  protected Dictionary<object, Func<bool, bool>> enabledAlterers = new Dictionary<object, Func<bool, bool>>();


  public ToggleDualAttribute(bool enabled = true) {
    this.enabled = enabled;
  }
  public ToggleDualAttribute(T value, T other, bool enabled = true) : base(value, other) {
    this.enabled = enabled;
  }


  internal void RegisterEnabledAlterer(Func<bool, bool> alterer) {
    if (!AttributeBase.allow) throw new SecurityException("Configuring alterers is only allowed inside the RegisterAttributeAlterers function!");
    var keyObject = new object();
    enabledAlterers.Add(keyObject, alterer);
    AttributeBase.keyTarget.Add(keyObject, this);
  }

  /// <summary> Internal use only. Attribute alterers are removed automatically. </summary>
  public override void RemoveAlterer(object key) {
    if (!AttributeBase.allow) throw new SecurityException("Configuring alterers is only allowed inside the RegisterAttributeAlterers function!");
    alterers.Remove(key);
    otherAlterers.Remove(key);
    enabledAlterers.Remove(key);
  }
}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ToggleDualAttribute<>))]
public class ToggleDualAttributeDrawer : PropertyDrawer {

  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

    using (PropertyScope(position, label, property, out label))
    using (RestoreLabelWidthScope())
    using (RestoreFieldWidthScope()) {

      var enabledProperty = property.FindPropertyRelative(nameof(ToggleDualAttribute<int>.enabled));
      var valueProperty = property.FindPropertyRelative("_value");
      var otherProperty = property.FindPropertyRelative("_other");

      var fieldInfo = GetFieldInfo(property);
      var labelAttribute = fieldInfo?.GetCustomAttributes(typeof(AttributeLabelsAttribute), false).FirstOrDefault() as AttributeLabelsAttribute;

      var noLabel = label.text is "" && label.image is null;
      var pos = position;
      pos.width = noLabel ? 0 : EditorGUIUtility.labelWidth;
      if (!noLabel) EditorGUI.LabelField(pos, label);

      using (new EditorGUI.IndentLevelScope(-EditorGUI.indentLevel)) {
        pos.xMin = pos.xMax + spacing;
        pos.width = 15 + spacing;
        EditorGUI.PropertyField(pos, enabledProperty, GUIContent.none);

        var valueLabel = new GUIContent(labelAttribute?.primaryLabel ?? "Value");
        EditorGUIUtility.labelWidth = GUI.skin.label.CalcSize(valueLabel).x - spacing;
        pos.xMin = pos.xMax + spacing;
        pos.width = (position.xMax - pos.xMin - 15 - spacing * 2) / 2 - spacing;
        EditorGUI.PropertyField(pos, valueProperty, valueLabel);

        var baseLabel = new GUIContent(labelAttribute?.secondaryLabel ?? "Other");
        EditorGUIUtility.labelWidth = GUI.skin.label.CalcSize(baseLabel).x - spacing;
        pos.xMin = pos.xMax + spacing;
        pos.xMax = position.xMax;
        EditorGUI.PropertyField(pos, otherProperty, baseLabel);
      }

    }

  }

}
#endif