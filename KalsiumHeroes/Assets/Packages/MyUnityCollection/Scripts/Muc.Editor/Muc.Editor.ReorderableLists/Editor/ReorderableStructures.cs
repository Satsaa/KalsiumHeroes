

#if UNITY_EDITOR
namespace Muc.Editor.ReorderableLists {

  using System;
  using System.Collections.Generic;
  using UnityEditor;
  using UnityEngine;


  internal class ReorderableStructures : ReorderableValues {


    protected float idealLabelWidth;

    public ReorderableStructures(SerializedProperty property) : base(property) { }

    //======================================================================

    protected override float GetElementHeight(SerializedProperty element, int elementIndex) {
      var properties = element.EnumerateChildProperties();
      return GetElementHeight(properties);
    }

    protected float GetElementHeight(IEnumerable<SerializedProperty> properties) {
      var spacing = EditorGUIUtility.standardVerticalSpacing;
      var height = 0f;

      idealLabelWidth = 0f;
      var labelStyle = EditorStyles.label;
      var labelContent = new GUIContent();

      var propertyCount = 0;
      foreach (var property in properties) {
        if (propertyCount++ > 0)
          height += spacing;

        height += GetPropertyHeight(property);

        labelContent.text = property.displayName;
        var minLabelWidth = labelStyle.CalcSize(labelContent).x;
        idealLabelWidth = Mathf.Max(idealLabelWidth, minLabelWidth);
      }
      idealLabelWidth += 8;
      return height;
    }

    //======================================================================

    protected override void DrawElement(Rect position, SerializedProperty element, int elementIndex, bool isActive) {
      var properties = element.EnumerateChildProperties();
      DrawElements(position, properties, elementIndex, isActive);
      if (elementIndex > 0) DrawHorizontalLine(position);
    }

    private static readonly GUIStyle eyeDropperHorizontalLine = "EyeDropperHorizontalLine";

    protected static void DrawHorizontalLine(Rect position) {
      if (IsRepaint()) {
        var style = eyeDropperHorizontalLine;
        position.yMin -= 3;
        position.xMin -= 15;
        position.height = 1;
        style.Draw(position, false, false, false, false);
      }
    }

    protected override float extraSpacing => 3;

    protected void DrawElements(Rect position, IEnumerable<SerializedProperty> properties, int elementIndex, bool isActive) {
      var spacing = EditorGUIUtility.standardVerticalSpacing;
      using (EditorUtil.LabelWidthScope(v => EditorGUIUtility.labelWidth - position.xMin + 19)) {
        var propertyCount = 0;
        foreach (var property in properties) {
          if (propertyCount++ > 0)
            position.y += spacing;

          position.height = GetPropertyHeight(property);
          PropertyField(position, property);
          position.y += position.height;
        }
      }
    }

  }

}
#endif