
#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Attribute<T> {

  [Tooltip("Primary value")]
  public T value;

  public Attribute() { }

  public Attribute(T value) {
    this.value = value;
  }
}


#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(Attribute<>))]
internal class AttributeDrawer : PropertyDrawer {

  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    using (new EditorGUI.PropertyScope(position, label, property)) {

      var originalLabelWidth = EditorGUIUtility.labelWidth;
      var originalFieldWidth = EditorGUIUtility.fieldWidth;

      var valueProperty = property.FindPropertyRelative(nameof(Attribute<int>.value));

      var noLabel = label.text is "" && label.image is null;

      var labelRect = position;
      labelRect.width = noLabel ? 0 : EditorGUIUtility.labelWidth;
      if (!noLabel) EditorGUI.LabelField(labelRect, label);

      var padding = EditorGUIUtility.standardVerticalSpacing;

      EditorGUIUtility.labelWidth = 35;
      var propPos = position;
      propPos.xMin = labelRect.xMax + padding;
      propPos.xMax = position.xMax;
      EditorGUI.PropertyField(propPos, valueProperty, new GUIContent("Value"));

      EditorGUIUtility.labelWidth = originalLabelWidth;
      EditorGUIUtility.fieldWidth = originalFieldWidth;
    }
  }

}

#endif
