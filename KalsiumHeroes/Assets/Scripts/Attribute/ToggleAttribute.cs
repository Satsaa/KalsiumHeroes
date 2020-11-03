
#if UNITY_EDITOR
using UnityEditor;
using static Muc.Editor.PropertyUtil;
using static Muc.Editor.EditorUtil;
#endif

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ToggleAttribute<T> : Attribute<T> {

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
public class ToggleAttributeDrawer : PropertyDrawer {

  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

    using (PropertyScope(position, label, property, out label))
    using (RestoreLabelWidthScope())
    using (RestoreFieldWidthScope()) {

      var enabledProperty = property.FindPropertyRelative(nameof(ToggleAttribute<int>.enabled));
      var valueProperty = property.FindPropertyRelative(nameof(Attribute<int>.value));

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

        pos.xMin = pos.xMax + spacing;
        pos.xMax = position.xMax;
        EditorGUI.PropertyField(pos, valueProperty, new GUIContent(labelAttribute?.primaryLabel ?? ""));
      }

    }

  }

}
#endif