
#if UNITY_EDITOR
using UnityEditor;
using static Muc.Editor.EditorUtil;
using static Muc.Editor.PropertyUtil;
#endif

using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
public class AttributeDrawer : PropertyDrawer {

  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

    using (PropertyScope(position, label, property, out label))
    using (RestoreLabelWidthScope())
    using (RestoreFieldWidthScope()) {

      var valueProperty = property.FindPropertyRelative(nameof(Attribute<int>.value));

      var fieldInfo = GetFieldInfo(property);
      var labelAttribute = fieldInfo?.GetCustomAttributes(typeof(AttributeLabelsAttribute), false).FirstOrDefault() as AttributeLabelsAttribute;

      var noLabel = label.text is "" && label.image is null;
      var pos = position;
      pos.width = noLabel ? 0 : EditorGUIUtility.labelWidth;
      if (!noLabel) EditorGUI.LabelField(pos, label);

      using (new EditorGUI.IndentLevelScope(-EditorGUI.indentLevel)) {
        EditorGUIUtility.labelWidth = 35;
        pos.xMin = pos.xMax + spacing;
        pos.xMax = position.xMax;
        EditorGUI.PropertyField(pos, valueProperty, new GUIContent(labelAttribute?.primaryLabel ?? ""));
      }

    }

  }

}
#endif
