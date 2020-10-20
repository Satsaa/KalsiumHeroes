
#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attribute<T> {

  [Tooltip("Reset value.")]
  public T baseValue;
  [Tooltip("Current value.")]
  public T value;

  public Attribute() { }

  public Attribute(T baseAndValue) {
    this.baseValue = this.value = baseAndValue;
  }
  public Attribute(T baseValue, T value) {
    this.baseValue = baseValue;
    this.value = value;
  }

  /// <summary> Sets the value to the baseValue </summary>
  public virtual void Reset() {
    value = baseValue;
  }

  public bool ValueEquals(Attribute<T> b) => b != null && value.Equals(b.value);
}

[System.Serializable]
public class ToggleAttribute<T> : Attribute<T> {

  public static implicit operator bool(ToggleAttribute<T> a) => !object.ReferenceEquals(a, null) && a.enabled;

  [Tooltip("Is this attribute enabled?")]
  public bool enabled = true;

  public ToggleAttribute(bool enabled = true) { this.enabled = enabled; }
  public ToggleAttribute(T baseAndValue, bool enabled = true) : base(baseAndValue) { this.enabled = enabled; }
  public ToggleAttribute(T baseValue, T value, bool enabled = true) : base(baseValue, value) { this.enabled = enabled; }

}


#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(Attribute<>))]
internal class AttributeDrawer : PropertyDrawer {

  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    using (new EditorGUI.PropertyScope(position, label, property)) {

      var originalLabelWidth = EditorGUIUtility.labelWidth;
      var originalFieldWidth = EditorGUIUtility.fieldWidth;

      var baseValue = property.FindPropertyRelative(nameof(Attribute<int>.baseValue));
      var value = property.FindPropertyRelative(nameof(Attribute<int>.value));


      var noLabel = label.text is "" && label.image is null;

      var labelRect = position;
      labelRect.width = noLabel ? 0 : EditorGUIUtility.labelWidth;
      if (!noLabel) EditorGUI.LabelField(labelRect, label);

      var padding = EditorGUIUtility.standardVerticalSpacing;
      var propWidth = (position.width - labelRect.width) / 2;

      var propPos1 = position;
      propPos1.xMin = labelRect.xMax + padding;
      propPos1.width = propWidth - padding;
      EditorGUIUtility.labelWidth = 35;
      EditorGUI.PropertyField(propPos1, baseValue, new GUIContent("Base"));

      var propPos2 = propPos1;
      propPos2.xMin = propPos1.xMax + padding;
      propPos2.width = propWidth - padding;
      EditorGUI.PropertyField(propPos2, value, new GUIContent("Value"));

      EditorGUIUtility.labelWidth = originalLabelWidth;
      EditorGUIUtility.fieldWidth = originalFieldWidth;
    }
  }

}


[CustomPropertyDrawer(typeof(ToggleAttribute<>))]
internal class ToggleAttributeDrawer : PropertyDrawer {

  public void OnEnable() {
  }

  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    using (new EditorGUI.PropertyScope(position, label, property)) {

      var originalLabelWidth = EditorGUIUtility.labelWidth;
      var originalFieldWidth = EditorGUIUtility.fieldWidth;

      var baseValue = property.FindPropertyRelative(nameof(ToggleAttribute<int>.baseValue));
      var value = property.FindPropertyRelative(nameof(ToggleAttribute<int>.value));
      var enabled = property.FindPropertyRelative(nameof(ToggleAttribute<int>.enabled));


      var noLabel = label.text is "" && label.image is null;

      var labelRect = position;
      labelRect.width = noLabel ? 0 : EditorGUIUtility.labelWidth;
      if (!noLabel) EditorGUI.LabelField(labelRect, label);

      var pausedRect = new Rect(position);
      if (!noLabel) pausedRect.xMin = pausedRect.xMin + EditorGUIUtility.labelWidth - 15 * (EditorGUI.indentLevel + 1);
      pausedRect.width = 15;
      EditorGUI.PropertyField(pausedRect, enabled, GUIContent.none);

      var padding = EditorGUIUtility.standardVerticalSpacing;
      var propWidth = (position.width - labelRect.width) / 2;

      var propPos1 = position;
      propPos1.xMin = labelRect.xMax + padding;
      propPos1.width = propWidth - padding;
      if (noLabel) propPos1.xMin += pausedRect.width + padding;
      EditorGUIUtility.labelWidth = 35;
      EditorGUI.PropertyField(propPos1, baseValue, new GUIContent("Base"));

      var propPos2 = propPos1;
      propPos2.xMin = propPos1.xMax + padding;
      propPos2.width = propWidth - padding;
      EditorGUI.PropertyField(propPos2, value, new GUIContent("Value"));

      EditorGUIUtility.labelWidth = originalLabelWidth;
      EditorGUIUtility.fieldWidth = originalFieldWidth;
    }
  }

}

#endif