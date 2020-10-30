
#if UNITY_EDITOR
using UnityEditor;
using static Muc.Editor.PropertyUtil;
using static Muc.Editor.EditorUtil;
#endif

using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[System.Serializable]
public class DualAttribute<T> : Attribute<T> {

  [Tooltip("Other value")]
  public T other;

  public DualAttribute() { }

  public DualAttribute(T value, T other) : base(value) {
    this.other = other;
  }

  /// <summary> Sets value to other. </summary>
  public void ResetValue() {
    value = other;
  }

  /// <summary> If value is larger than other, sets value to other. T must be castable to IComparable. </summary>
  public void LimitValue() {
    if (((IComparable)value).CompareTo((IComparable)other) > 0) {
      value = other;
    }
  }
}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(DualAttribute<>))]
internal class DualAttributeDrawer : PropertyDrawer {

  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

    using (PropertyScope(position, label, property, out label))
    using (LabelWidthScope())
    using (FieldWidthScope()) {

      var valueProperty = property.FindPropertyRelative(nameof(DualAttribute<int>.value));
      var baseProperty = property.FindPropertyRelative(nameof(DualAttribute<int>.other));

      var fieldInfo = GetFieldInfo(property);
      var labelAttribute = fieldInfo?.GetCustomAttributes(typeof(AttributeLabelsAttribute), false).FirstOrDefault() as AttributeLabelsAttribute;

      var noLabel = label.text is "" && label.image is null;
      var pos = position;
      pos.width = noLabel ? 0 : EditorGUIUtility.labelWidth;
      if (!noLabel) EditorGUI.LabelField(pos, label);

      using (new EditorGUI.IndentLevelScope(-EditorGUI.indentLevel)) {
        var valueLabel = new GUIContent(labelAttribute?.primaryLabel ?? "Value");
        EditorGUIUtility.labelWidth = GUI.skin.label.CalcSize(valueLabel).x - spacing;
        pos.xMin = pos.xMax + spacing;
        pos.width = (position.xMax - pos.xMin) / 2 - spacing;
        EditorGUI.PropertyField(pos, valueProperty, valueLabel);

        var baseLabel = new GUIContent(labelAttribute?.secondaryLabel ?? "Other");
        EditorGUIUtility.labelWidth = GUI.skin.label.CalcSize(baseLabel).x - spacing;
        pos.xMin = pos.xMax + spacing;
        pos.xMax = position.xMax;
        EditorGUI.PropertyField(pos, baseProperty, baseLabel);
      }

    }

  }

}
#endif