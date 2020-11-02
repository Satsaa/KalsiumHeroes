

#if UNITY_EDITOR
namespace Muc.Editor.ReorderableLists {

  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Reflection;
  using UnityEditor;
  using UnityEditorInternal;
  using UnityEngine;

  using static Muc.Editor.EditorUtil;


  public class ReorderableDrawer : ArrayDrawer {

    public delegate void BackgroundColorDelegate(SerializedProperty array, int index, ref Color backgroundColor);

    //======================================================================

    public override bool CanCacheInspectorGUI(SerializedProperty property) {
      return true;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
      var reorderableListOfValues = GetReorderableList(fieldInfo, property);

      Debug.Assert(reorderableListOfValues.serializedProperty.propertyPath == property.propertyPath);

      try {
        return reorderableListOfValues.GetHeight(label);
      } catch (Exception ex) {
        Debug.LogException(ex);
        return 0f;
      }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      // Add property scope
      var propertyPos = position;
      propertyPos.height = 20 + (property.isExpanded ? 1 : 0);
      using (PropertyScope(propertyPos, label, property, out label)) {
        var reorderableList = GetReorderableList(fieldInfo, property);
        reorderableList.UpdateLabel(label);
        reorderableList.onSelectCallback += OnSelectCallback;
        reorderableList.DoGUI(position);
        reorderableList.onSelectCallback -= OnSelectCallback;
      }
    }

    //======================================================================

    private void OnSelectCallback(ReorderableList list) {
      var array = list.serializedProperty;
      var index = list.index;
    }

    //======================================================================

    private class ReorderableListMap : Dictionary<string, ReorderableValues> {
      public ReorderableValues Find(string key) {
        var reorderableList = default(ReorderableValues);
        base.TryGetValue(key, out reorderableList);
        return reorderableList;
      }
    }

    private readonly ReorderableListMap reorderableListMap = new ReorderableListMap();
    private ReorderableValues mostRecentReorderableList;
    private string mostRecentPropertyPath;

    private ReorderableValues GetReorderableList(FieldInfo fieldInfo, SerializedProperty property) {
      var propertyPath = property.propertyPath;

      if (mostRecentReorderableList != null) {
        if (mostRecentPropertyPath == propertyPath) {
          mostRecentReorderableList.serializedProperty = property;
          return mostRecentReorderableList;
        }
      }

      mostRecentReorderableList = reorderableListMap.Find(propertyPath);

      if (mostRecentReorderableList == null) {
        var reorderableList = CreateReorderableList(fieldInfo, property);
        reorderableListMap.Add(propertyPath, reorderableList);
        mostRecentReorderableList = reorderableList;
      } else {
        mostRecentReorderableList.serializedProperty = property;
      }

      mostRecentPropertyPath = propertyPath;

      return mostRecentReorderableList;
    }

    private ReorderableValues CreateReorderableList(FieldInfo fieldInfo, SerializedProperty property) {
      var listType = fieldInfo.FieldType;

      var elementType = GetArrayOrListElementType(listType);

      var elementIsValue =
          elementType.IsEnum ||
          elementType.IsPrimitive ||
          elementType == typeof(string) ||
          elementType == typeof(Color) ||
          elementType == typeof(LayerMask) ||
          elementType == typeof(Vector2) ||
          elementType == typeof(Vector3) ||
          elementType == typeof(Vector4) ||
          elementType == typeof(Rect) ||
          elementType == typeof(AnimationCurve) ||
          elementType == typeof(Bounds) ||
          elementType == typeof(Gradient) ||
          elementType == typeof(Quaternion) ||
          elementType == typeof(Vector2Int) ||
          elementType == typeof(Vector3Int) ||
          elementType == typeof(RectInt) ||
          elementType == typeof(BoundsInt);

      if (elementIsValue) {
        return new ReorderableValues(property, listType, elementType);
      }

      var elementIsUnityEngineObject = typeof(UnityEngine.Object).IsAssignableFrom(elementType);

      if (elementIsUnityEngineObject) {
        return new ReorderableValues(property, listType, elementType);
      }

      var elementPropertyDrawerType = GetDrawerTypeForType(elementType);
      if (elementPropertyDrawerType == null) {
        var elementIsStruct =
            elementType.IsValueType &&
            !elementType.IsEnum &&
            !elementType.IsPrimitive;

        var elementIsClass = elementType.IsClass;

        if (elementIsStruct || elementIsClass) {
          return new ReorderableStructures(property, listType, elementType);
        }
      }

      return new ReorderableValues(property, listType, elementType);

    }

    //======================================================================

    private delegate Type GetArrayOrListElementTypeDelegate(Type listType);

    private static readonly GetArrayOrListElementTypeDelegate GetArrayOrListElementType =
      (GetArrayOrListElementTypeDelegate)Delegate.CreateDelegate(
        typeof(GetArrayOrListElementTypeDelegate),
        null,
        typeof(PropertyDrawer)
          .Assembly
          .GetType("UnityEditor.EditorExtensionMethods")
          .GetMethod(
            "GetArrayOrListElementType",
            BindingFlags.NonPublic | BindingFlags.Static
          )
      );

    //======================================================================

    private delegate Type GetDrawerTypeForTypeDelegate(Type type);

    private static readonly GetDrawerTypeForTypeDelegate GetDrawerTypeForType =
      (GetDrawerTypeForTypeDelegate)Delegate.CreateDelegate(
        typeof(GetDrawerTypeForTypeDelegate),
        null,
        typeof(PropertyDrawer)
          .Assembly
          .GetType("UnityEditor.ScriptAttributeUtility")
          .GetMethod(
            "GetDrawerTypeForType",
            BindingFlags.NonPublic | BindingFlags.Static
          )
      );

  }

}
#endif