
#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResetAttribute<T> : Attribute<T> {

  [Tooltip("Secondary value")]
  public T other;

  public ResetAttribute() { }

  public ResetAttribute(T value, T other) : base(value) {
    this.other = other;
  }
}


#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(ResetAttribute<>))]
internal class ResetAttributeDrawer : PropertyDrawer {

  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    using (new EditorGUI.PropertyScope(position, label, property)) {

      var originalLabelWidth = EditorGUIUtility.labelWidth;
      var originalFieldWidth = EditorGUIUtility.fieldWidth;

      var valueProperty = property.FindPropertyRelative(nameof(ResetAttribute<int>.value));
      var otherProperty = property.FindPropertyRelative(nameof(ResetAttribute<int>.other));

      var noLabel = label.text is "" && label.image is null;

      var labelRect = position;
      labelRect.width = noLabel ? 0 : EditorGUIUtility.labelWidth;
      if (!noLabel) EditorGUI.LabelField(labelRect, label);

      var padding = EditorGUIUtility.standardVerticalSpacing;

      EditorGUIUtility.labelWidth = 35;
      var valuePos = position;
      valuePos.xMin = labelRect.xMax + padding;
      valuePos.xMax = position.xMax;
      valuePos.width /= 2;
      valuePos.width -= padding;
      EditorGUI.PropertyField(valuePos, valueProperty, new GUIContent("Value"));

      EditorGUIUtility.labelWidth = 35;
      var otherPos = position;
      otherPos.xMin = valuePos.xMax + padding * 2;
      otherPos.xMax = position.xMax;
      EditorGUI.PropertyField(otherPos, otherProperty, new GUIContent("Other"));

      EditorGUIUtility.labelWidth = originalLabelWidth;
      EditorGUIUtility.fieldWidth = originalFieldWidth;
    }
  }

}

#endif