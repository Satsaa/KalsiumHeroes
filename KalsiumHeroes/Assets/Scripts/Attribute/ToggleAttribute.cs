
#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ToggleAttribute<T> : Attribute<T> {

  public static implicit operator bool(ToggleAttribute<T> a) => !object.ReferenceEquals(a, null) && a.enabled;

  [Tooltip("Attribute is enabled?")]
  public bool enabled;


  public ToggleAttribute(bool enabled = true) {
    this.enabled = enabled;
  }
  public ToggleAttribute(T value, bool enabled = true) : base(value) {
    this.enabled = enabled;
  }
}


#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(ToggleAttribute<>))]
internal class ToggleAttributeDrawer : PropertyDrawer {

  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    using (new EditorGUI.PropertyScope(position, label, property)) {

      var originalLabelWidth = EditorGUIUtility.labelWidth;
      var originalFieldWidth = EditorGUIUtility.fieldWidth;

      var enabledProperty = property.FindPropertyRelative(nameof(ToggleAttribute<int>.enabled));
      var valueProperty = property.FindPropertyRelative(nameof(Attribute<int>.value));

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
      if (noLabel) valuePos.xMin = enabledRect.xMax + padding;
      EditorGUI.PropertyField(valuePos, valueProperty, new GUIContent("Value"));

      EditorGUIUtility.labelWidth = originalLabelWidth;
      EditorGUIUtility.fieldWidth = originalFieldWidth;
    }
  }

}

#endif