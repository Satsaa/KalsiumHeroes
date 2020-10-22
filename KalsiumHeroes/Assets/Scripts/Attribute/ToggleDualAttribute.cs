
#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ToggleDualAttribute<T> : ResetAttribute<T> {

  public static implicit operator bool(ToggleDualAttribute<T> a) => !object.ReferenceEquals(a, null) && a.enabled;

  [Tooltip("Attribute is enabled?")]
  public bool enabled;

  public ToggleDualAttribute(bool enabled = true) {
    this.enabled = enabled;
  }
  public ToggleDualAttribute(T value, T other, bool enabled = true) : base(value, other) {
    this.enabled = enabled;
  }
}


#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(ToggleDualAttribute<>))]
internal class ToggleDualAttributeDrawer : PropertyDrawer {

  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    using (new EditorGUI.PropertyScope(position, label, property)) {

      var originalLabelWidth = EditorGUIUtility.labelWidth;
      var originalFieldWidth = EditorGUIUtility.fieldWidth;

      var enabledProperty = property.FindPropertyRelative(nameof(ToggleDualAttribute<int>.enabled));
      var valueProperty = property.FindPropertyRelative(nameof(ToggleDualAttribute<int>.value));
      var resetProperty = property.FindPropertyRelative(nameof(ToggleDualAttribute<int>.other));

      var noLabel = label.text is "" && label.image is null;

      var labelRect = position;
      labelRect.width = noLabel ? 0 : EditorGUIUtility.labelWidth;
      if (!noLabel) EditorGUI.LabelField(labelRect, label);

      var padding = EditorGUIUtility.standardVerticalSpacing;

      var enabledRect = position;
      enabledRect.xMin = labelRect.xMax + padding;
      enabledRect.width = 15 + padding;
      EditorGUI.PropertyField(enabledRect, enabledProperty, GUIContent.none);

      EditorGUIUtility.labelWidth = 35;
      var valuePos = position;
      valuePos.xMin = enabledRect.xMax + padding;
      valuePos.xMax = position.xMax;
      valuePos.width /= 2;
      valuePos.width -= padding;
      EditorGUI.PropertyField(valuePos, valueProperty, new GUIContent("Value"));

      EditorGUIUtility.labelWidth = 35;
      var resetPos = position;
      resetPos.xMin = valuePos.xMax + padding * 2;
      resetPos.xMax = position.xMax;
      EditorGUI.PropertyField(resetPos, resetProperty, new GUIContent("Other"));

      EditorGUIUtility.labelWidth = originalLabelWidth;
      EditorGUIUtility.fieldWidth = originalFieldWidth;
    }
  }

}

#endif