
#if UNITY_EDITOR
using UnityEditor;
using static Muc.Editor.EditorUtil;
using static Muc.Editor.PropertyUtil;
#endif

using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class ToggleDualAttribute<T> : DualAttribute<T> {

  public static implicit operator bool(ToggleDualAttribute<T> a) => !object.ReferenceEquals(a, null) && a.enabled;

  [Tooltip("Attribute is enabled?")]
  public bool enabled;

  public ToggleDualAttribute(bool enabled = true) {
    this.enabled = enabled;
  }
  public ToggleDualAttribute(T value, T baseValue, bool enabled = true) : base(value, baseValue) {
    this.enabled = enabled;
  }
}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ToggleDualAttribute<>))]
internal class ToggleDualAttributeDrawer : PropertyDrawer {

  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

    using (PropertyScope(position, label, property, out label))
    using (LabelWidthScope())
    using (FieldWidthScope()) {

      var enabledProperty = property.FindPropertyRelative(nameof(ToggleDualAttribute<int>.enabled));
      var valueProperty = property.FindPropertyRelative(nameof(ToggleDualAttribute<int>.value));
      var baseProperty = property.FindPropertyRelative(nameof(ToggleDualAttribute<int>.other));

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
        EditorGUI.PropertyField(pos, baseProperty, baseLabel);
      }

    }

  }

}
#endif