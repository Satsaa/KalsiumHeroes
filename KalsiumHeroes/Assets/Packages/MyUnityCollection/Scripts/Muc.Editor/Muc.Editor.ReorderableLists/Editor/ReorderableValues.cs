

#if UNITY_EDITOR
namespace Muc.Editor.ReorderableLists {

  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Linq;
  using System.Reflection;
  using System.Text.RegularExpressions;
  using UnityEditor;
  using UnityEditorInternal;
  using UnityEngine;

  using static PropertyUtil;
  using static EditorUtil;


  internal class ReorderableValues : ReorderableList {

    public SerializedProperty prop => serializedProperty;
    public readonly Type listType;
    public readonly Type elementBaseType;

    public readonly bool isReferenceList;
    public readonly bool isUnityObjectList;

    protected static readonly new Defaults defaultBehaviours = new Defaults();

    //======================================================================

    public ReorderableValues(SerializedProperty primaryProperty)
      : base(primaryProperty.serializedObject, primaryProperty.Copy(), true, true, true, true) {

      var value = GetFirstValue<IList>(primaryProperty);
      listType = value.GetType();
      elementBaseType = listType.IsArray ? listType.GetElementType() : listType.GetGenericArguments()[0];
      isUnityObjectList = typeof(UnityEngine.Object).IsAssignableFrom(elementBaseType);

      this.isReferenceList = primaryProperty.arrayElementType == "managedReference<>";
      footerHeight = 0;

      drawHeaderCallback = DrawHeaderCallback;
      drawFooterCallback = DrawFooterCallback;
      elementHeightCallback = ElementHeightCallback;
      drawElementCallback = DrawElementCallback;
      drawElementBackgroundCallback = DrawElementBackgroundCallback;
      drawNoneElementCallback = DrawEmptyElementCallback;

      onCanAddCallback = CanAdd;
      onAddCallback = OnAddCallback;
      onCanRemoveCallback = CanRemove;
      onRemoveCallback = OnRemoveCallback;
    }

    //======================================================================

    public float GetHeight(GUIContent label) {
      UpdateLabel(label);
      if (!prop.isExpanded) return 20;
      UpdateElementHeights();
      var height = GetHeight();
      return height + (count < 1 ? -1 : -0);
    }

    public virtual void DoGUI(Rect position) {
      onNextGUIFrame?.Invoke();
      onNextGUIFrame = null;

      position = EditorGUI.IndentedRect(position);

      using (new EditorGUI.IndentLevelScope(-EditorGUI.indentLevel)) {
        try {
          if (prop.isExpanded) {
            if (count < 1) {
              DoEmptyList(position);
            } else {
              DoList(position);
            }
          } else {
            index = -1;
            DoCollapsedListBackground(position);
          }
        } finally {
          DoHeader(position);
          if (displayAdd || displayRemove) {
            var footerRect = position;
            footerRect.yMin += 2;
            DoFooter(footerRect, this);
          }
        }
      }
    }

    //======================================================================

    protected int ClampIndex(int index, int plusMax = -1) {
      var max = Mathf.Max(0, prop.arraySize + plusMax);
      return Mathf.Clamp(index, 0, max);
    }

    //======================================================================

    protected bool HasMultipleArraySizes() {
      var vals = GetValues<IList>(prop);
      var firstLength = vals.First().Count;
      var sameLengths = vals.Aggregate(true, (b, v) => v == null ? b : b && v.Count == firstLength);
      return sameLengths;
    }

    //======================================================================

    protected IEnumerable<int> GetMultipleArraySizes() {
      var vals = GetValues<IList>(prop);
      foreach (var val in vals) {
        yield return val.Count;
      }
    }

    //======================================================================

    private void DoCollapsedListBackground(Rect position) {
      var headerRect = position;
      headerRect.height = headerHeight;

      if (showDefaultBackground && IsRepaint()) {
        defaultBehaviours.DrawHeaderBackground(headerRect);
      }
    }

    //======================================================================

    private bool CanAdd(ReorderableList list) {
      if (!prop.serializedObject.isEditingMultipleObjects) return true;
      return HasMultipleArraySizes();
    }

    private void OnAddCallback(ReorderableList list) {
      prop.isExpanded = true;
      InsertElement(index < 0 ? count : index);
    }

    private bool CanRemove(ReorderableList list) {
      return prop.isExpanded && index >= 0 && !prop.hasMultipleDifferentValues;
    }

    private void OnRemoveCallback(ReorderableList list) {
      DeleteElement(index);
    }

    //======================================================================

    [Serializable]
    private class ClipboardElement {
      public string type;
      public string assemblyQualifiedName;
      public string json;
      public int instanceId;

      public static ClipboardElement Deserialize(string s) {
        try {
          return JsonUtility.FromJson<ClipboardElement>(s);
        } catch {
          return null;
        }
      }

      public string Serialize() {
        return JsonUtility.ToJson(this);
      }
    }

    private ClipboardElement CopyElementContent(int elementIndex) {
      if (elementIndex < 0) throw new IndexOutOfRangeException("Index must be non-negative.");

      var arrayObj = (IList)prop.GetObject();
      var elementObj = arrayObj[elementIndex];
      var elementType = elementObj.GetType();
      var elementJson =
        elementType.IsPrimitive || elementType == typeof(string) ?
        elementObj.ToString() :
        JsonUtility.ToJson(elementObj);

      var clipboardElement = new ClipboardElement();
      clipboardElement.type = elementType.FullName;
      clipboardElement.assemblyQualifiedName = elementType.AssemblyQualifiedName;
      clipboardElement.json = elementJson;
      if (elementObj is UnityEngine.Object unityObject) clipboardElement.instanceId = unityObject.GetInstanceID();
      return clipboardElement;
    }

    private void CopyElementToClipboard(int elementIndex) {
      if (elementIndex < 0) return;
      EditorGUIUtility.systemCopyBuffer = CopyElementContent(elementIndex).Serialize();
    }

    private void CutElement(int elementIndex) {
      if (elementIndex < 0) return;

      CopyElementToClipboard(elementIndex);
      DeleteElement(elementIndex);
    }

    private bool CanCopy(int elementIndex) {
      var elementProperty = prop.GetArrayElementAtIndex(elementIndex);
      return !elementProperty.hasMultipleDifferentValues;
    }

    private bool CanPaste(ClipboardElement clipboardElement, int elementIndex) {
      if (clipboardElement == null) return false;
      if (elementIndex > prop.arraySize) return false;
      var arrayObj = (IList)prop.GetObject();
      var arrayType = arrayObj.GetType();
      var elementType = arrayType.IsArray ? arrayType.GetElementType() : arrayType.GetGenericArguments()[0];
      var clipboardType = Type.GetType(clipboardElement.assemblyQualifiedName);
      return elementType.IsAssignableFrom(clipboardType);
    }

    private void PasteElement(int elementIndex, ClipboardElement clipboardElement) {
      if (elementIndex < 0) return;

      if (clipboardElement == null) return;

      var serializedObject = prop.serializedObject;
      var targetObject = serializedObject.targetObject;
      Undo.RecordObject(targetObject, "Paste value");

      if (elementIndex >= prop.arraySize)
        prop.arraySize = elementIndex + 1;

      var arrayObjs = GetValues<IList>(prop);
      var arrayType = prop.GetObject().GetType();
      var elementProperty = prop.GetArrayElementAtIndex(elementIndex);
      var elementType = arrayType.IsArray ? arrayType.GetElementType() : arrayType.GetGenericArguments()[0];
      var elementJson = clipboardElement.json;
      var elementInstanceId = clipboardElement.instanceId;

      if (elementType.IsPrimitive || elementType == typeof(string)) {
        object newValue;
        switch (GetFirstValue<object>(elementProperty)) {
          case String _:
            newValue = elementJson;
            break;
          case Char _:
            newValue = Char.Parse(elementJson);
            break;
          case Boolean _:
            newValue = Boolean.Parse(elementJson);
            break;
          case Single _:
          case Double _:
          case Decimal _:
            newValue = Decimal.Parse(elementJson);
            break;
          case UInt64 _:
            newValue = UInt64.Parse(elementJson);
            break;
          default: // Other number type
            newValue = Int64.Parse(elementJson);
            break;
        }
        var converted = Convert.ChangeType(newValue, elementType);
        SetValueNoRecord(elementProperty, converted);
      } else if (typeof(UnityEngine.Object).IsAssignableFrom(elementType)) {
        var fromId = EditorUtility.InstanceIDToObject(elementInstanceId);
        if (fromId != null) SetValueNoRecord(elementProperty, fromId);
      } else {
        var fromJson = JsonUtility.FromJson(elementJson, elementType);
        if (fromJson != null) SetValueNoRecord(elementProperty, fromJson);
      }

      serializedObject.Update();
      GUI.changed = true;
    }

    //======================================================================

    private static MemberInfo[] GetFirstMemberInHierarchy(Type type, string name, MemberTypes memberTypes, BindingFlags bindingAttr) {
      MemberInfo[] res;
      do {
        res = type.GetMember(name, memberTypes, bindingAttr);
        if (res.Length != 0) return res;
      } while ((type = type.BaseType) != null);
      return res;
    }


    protected static Type GetManagedReferenceType(SerializedProperty property) {
      var typeStrings = property.managedReferenceFullTypename.Split(' ');
      var fullTypeName = typeStrings[1];
      var assemblyName = typeStrings[0];
      var assembly = Assembly.Load(assemblyName);
      var type = assembly.GetType(fullTypeName, true);
      return type;
    }

    protected virtual void InsertElement(int elementIndex) {
      elementIndex = ClampIndex(elementIndex);

      var serializedObject = prop.serializedObject;
      prop.InsertArrayElementAtIndex(elementIndex);

      var type = prop.arrayElementType;

      // Set correct default values for first element
      if (prop.arraySize == 1) {
        var element = prop.GetArrayElementAtIndex(elementIndex);
        var elPropType = element.propertyType;
        if (isReferenceList) {

          var instance = InstaniateType(elementBaseType);
          if (instance != null) {
            serializedObject.ApplyModifiedProperties();
            element.managedReferenceValue = instance;
          }
        } else if (elPropType == SerializedPropertyType.Generic) {
          var instance = InstaniateType(elementBaseType);
          if (instance != null) {
            serializedObject.ApplyModifiedProperties();
            SetValueNoRecord(element, instance);
          }
        }
      } else if (isReferenceList) {
        // Copy previous element for new elements
        var element = prop.GetArrayElementAtIndex(elementIndex);

        var copyIndex = elementIndex == 0 ? 1 : elementIndex - 1;
        var copyElement = prop.GetArrayElementAtIndex(copyIndex);
        var copyElementType = GetManagedReferenceType(copyElement);

        object instance = InstaniateType(copyElementType);
        if (instance != null) {
          serializedObject.ApplyModifiedProperties();
          element.managedReferenceValue = instance;
        }
      }

      serializedObject.ApplyModifiedProperties();
      index = ClampIndex(elementIndex + 1);
      GUI.changed = true;
    }

    private static object InstaniateType(Type type) {
      object instance = null;
      try {
        instance = Activator.CreateInstance(type, true);
      } catch (SystemException) {
        try {
          instance = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(type);
        } catch (SystemException) {
        } catch (Exception) {
          throw;
        }
      }
      return instance;
    }

    private static Type GetSerializedPropertyType(SerializedProperty property) {
      var parentType = property.serializedObject.targetObject.GetType();
      return GetTypeByPath(parentType, property.propertyPath);
    }

    private static Type GetTypeByPath(Type type, string path) {
      path = path.Replace(".Array.data[", "[");
      var currentType = type;
      FieldInfo field = null;
      foreach (var token in path.Split('.')) {
        if (token.Contains("[")) {
          var elementName = token.Substring(0, token.IndexOf("["));
          var bracketPos = token.IndexOf("[");
          var index = System.Convert.ToInt32(token.Substring(bracketPos + 1, token.Length - (bracketPos + 2)));

          field = GetFirstMemberInHierarchy(currentType, elementName, MemberTypes.Field, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault() as FieldInfo;

          if (field is null) return null;
          var listType = field.FieldType;
          if (listType.IsArray) {
            currentType = listType.GetElementType();
          } else {
            currentType = listType.GenericTypeArguments[0];
          }

        } else {
          field = currentType.GetField(token);

          if (field is null) return null;
          currentType = field.FieldType;
        }
      }
      if (field is null) return null;
      return currentType;
    }

    //======================================================================

    protected virtual void DeleteElement(int elementIndex) {
      if (elementIndex < 0 || elementIndex >= count) return;

      var prop = this.prop;
      var serializedObject = prop.serializedObject;

      var element = prop.GetArrayElementAtIndex(elementIndex);
      var oldSubassets = element.FindReferencedSubassets();
      var preDelSize = prop.arraySize;
      prop.DeleteArrayElementAtIndex(elementIndex);
      if (isUnityObjectList && preDelSize == prop.arraySize) { // Unity Objects get set to none first...
        prop.DeleteArrayElementAtIndex(elementIndex);
      }
      if (preDelSize != prop.arraySize) { // Make sure deletion actually happened
        index = Math.Min(elementIndex, prop.arraySize - 1);
        if (oldSubassets.Any()) {
          Debug.Log("Had sub assets!");
          serializedObject.ApplyModifiedPropertiesWithoutUndo();
          serializedObject.DestroyUnreferencedSubassets(oldSubassets);
        } else {
          serializedObject.ApplyModifiedProperties();
          serializedObject.Update();
        }
      }
      elementIndex = ClampIndex(elementIndex);
      GUI.changed = true;
    }

    //======================================================================

    protected virtual float GetElementHeight(SerializedProperty element, int elementIndex) {
      return GetPropertyHeight(element, GUIContent.none);
    }

    protected virtual void DrawElement(Rect position, SerializedProperty element, int elementIndex, bool isActive) {
      PropertyField(position, element, GUIContent.none);
    }

    //======================================================================

    private void DrawElementBackground(Rect position, SerializedProperty element, int elementIndex, bool isActive, bool isFocused) {
      defaultBehaviours.DrawElementBackground(position, elementIndex, isActive, isFocused, draggable: true);

    }

    //======================================================================

    private Action onNextGUIFrame;

    protected void OnNextGUIFrame(Action action) {
      onNextGUIFrame += action;
    }

    //======================================================================

    protected static readonly GUIContent cutLabel = new GUIContent("Cut");
    protected static readonly GUIContent copyLabel = new GUIContent("Copy");
    protected static readonly GUIContent pasteLabel = new GUIContent("Paste");
    protected static readonly GUIContent deleteLabel = new GUIContent("Delete");

    protected virtual void PopulateElementContextMenu(GenericMenu menu, int elementIndex) {
      var prop = this.prop;
      var serializedObject = prop.serializedObject;
      var canCopy = CanCopy(elementIndex);
      if (canCopy) {
        menu.AddItem(cutLabel, false, () => OnNextGUIFrame(() => CutElement(elementIndex)));
        menu.AddItem(copyLabel, false, () => CopyElementToClipboard(elementIndex));
      } else {
        menu.AddDisabledItem(cutLabel);
        menu.AddDisabledItem(copyLabel);
      }
      var content = ClipboardElement.Deserialize(EditorGUIUtility.systemCopyBuffer);
      var canPaste = CanPaste(content, elementIndex);
      if (canPaste) menu.AddItem(pasteLabel, false, () => OnNextGUIFrame(() => PasteElement(elementIndex, content)));
      else menu.AddDisabledItem(pasteLabel);
    }

    //======================================================================

    protected float GetPropertyHeight(SerializedProperty property) {
      return EditorGUI.GetPropertyHeight(property, includeChildren: true);
    }

    protected float GetPropertyHeight(SerializedProperty property, GUIContent label) {
      return EditorGUI.GetPropertyHeight(property, label, includeChildren: true);
    }

    //======================================================================

    protected void PropertyField(Rect position, SerializedProperty property) {
      EditorGUI.PropertyField(position, property, includeChildren: true);
    }

    protected void PropertyField(Rect position, SerializedProperty property, GUIContent label) {
      EditorGUI.PropertyField(position, property, label, includeChildren: true);
    }

    //======================================================================

    protected static readonly GUIStyle ContextMenuButtonStyle = "Button";

    protected static bool IsRepaint() {
      var current = Event.current;
      return current != null && current.type == EventType.Repaint;
    }

    //======================================================================

    private void DoHeader(Rect position) {
      defaultBehaviours.DrawHeaderBackground(position);

      var foldoutRect = position;
      foldoutRect.y++;
      foldoutRect.xMin += 15;
      foldoutRect.height = lineHeight;
      foldoutRect.width -= 50;
      prop.isExpanded = EditorGUI.Foldout(foldoutRect, prop.isExpanded, label, true);
    }

    //======================================================================

    private void DoEmptyList(Rect position) {
      // draw the background in repaint
      if (showDefaultBackground && Event.current.type == EventType.Repaint)
        defaultBehaviours.boxBackground.Draw(position, false, false, false, false);

      var elementRect = position;
      var elementContentRect = position;

      // draw the background
      if (drawElementBackgroundCallback == null)
        defaultBehaviours.DrawElementBackground(elementRect, -1, false, false, false);
      else
        drawElementBackgroundCallback(elementRect, -1, false, false);

      elementContentRect = elementRect;
      elementContentRect.xMin += Defaults.padding;
      elementContentRect.xMax -= Defaults.padding;
      elementContentRect.y += 10;
      DrawEmptyElementCallback(elementContentRect);
    }

    //======================================================================

    private static GUIContent iconToolbarPlus = EditorGUIUtility.TrIconContent("Toolbar Plus", "Add to list");
    private static GUIContent iconToolbarPlusMore = EditorGUIUtility.TrIconContent("Toolbar Plus More", "Choose to add to list");
    private static GUIContent iconToolbarMinus = EditorGUIUtility.TrIconContent("Toolbar Minus", "Remove selection or last element from list");
    private readonly GUIStyle preButton = "RL FooterButton";

    // draw the default footer
    public void DoFooter(Rect rect, ReorderableList list) {
      float rightEdge = rect.xMax;
      float leftEdge = rightEdge - 8f;
      if (list.displayAdd)
        leftEdge -= 25;
      if (list.displayRemove)
        leftEdge -= 25;
      rect = new Rect(leftEdge, rect.y, rightEdge - leftEdge, rect.height);
      Rect addRect = new Rect(leftEdge + 4, rect.y, 25, 16);
      Rect removeRect = new Rect(rightEdge - 29, rect.y, 25, 16);
      if (list.displayAdd) {
        using (DisabledScope(v => v || (list.onCanAddCallback != null && !list.onCanAddCallback(list)))) {
          if (GUI.Button(addRect, list.onAddDropdownCallback != null ? iconToolbarPlusMore : iconToolbarPlus, preButton)) {
            if (list.onAddDropdownCallback != null)
              list.onAddDropdownCallback(addRect, list);
            else if (list.onAddCallback != null)
              list.onAddCallback(list);
            else
              InsertElement(index < 0 ? count - 1 : index);

            list.onChangedCallback?.Invoke(list);
          }
        }
      }
      if (list.displayRemove) {
        using (DisabledScope(v => v || (
            list.index < 0 || list.index >= list.count ||
            (list.onCanRemoveCallback != null && !list.onCanRemoveCallback(list)))
          )
        ) {
          if (GUI.Button(removeRect, iconToolbarMinus, preButton)) {
            if (list.onRemoveCallback == null) {
              DeleteElement(index);
            } else
              list.onRemoveCallback(list);

            list.onChangedCallback?.Invoke(list);
          }
        }
      }
    }

    //======================================================================

    private GUIContent label = new GUIContent();

    internal void UpdateLabel(GUIContent source) {
      this.label.image = source.image;
      this.label.tooltip = string.IsNullOrEmpty(source.tooltip) ? prop.tooltip : source.tooltip;
      if (prop.serializedObject.isEditingMultipleObjects) {
        if (!HasMultipleArraySizes()) {
          var vals = GetValues<IList>(prop);
          var lengthString = vals.Aggregate("", (b, v) => v == null ? b : b + $"{v.Count}, ");
          lengthString = lengthString.Substring(0, lengthString.Length - 2);
          this.label.text = $"{source.text ?? string.Empty} ({lengthString})";
          return;
        }
      }
      this.label.text = $"{source.text ?? string.Empty} ({prop.arraySize})";
    }

    //======================================================================

    private readonly List<float> elementHeights = new List<float>();

    private void UpdateElementHeights() {
      var arraySize = GetMultipleArraySizes().Min();
      elementHeights.Clear();
      elementHeights.Capacity = arraySize;
      for (int i = 0; i < arraySize; i++) elementHeights.Add(0);

      if (prop.isExpanded) {
        for (int i = 0; i < arraySize; i++) {
          var element = prop.GetArrayElementAtIndex(i);
          var elementHeight = GetElementHeight(element, i);
          elementHeights[i] += elementHeight;
        }
        for (int i = 0; i < arraySize; i++) {
          var elementHeight = elementHeights[i];
          elementHeights[i] = AddElementPadding(elementHeight);
        }
      }
    }

    //======================================================================

    private void DrawHeaderCallback(Rect position) {
      // DoGUI draws the header content after the list is drawn
    }

    private void DrawFooterCallback(Rect position) {
      // Drawn in header
    }

    private float ElementHeightCallback(int elementIndex) {
      if (elementIndex >= elementHeights.Count) return EditorGUIUtility.singleLineHeight;
      return elementHeights[elementIndex];
    }

    private void DrawElementCallback(Rect position, int elementIndex, bool isActive, bool isFocused) {
      var primaryProperty = prop;
      if (primaryProperty.isExpanded) {
        RemoveElementPadding(ref position);
        DrawElementRows(position, elementIndex, isActive);
      }
    }

    private void DrawElementRows(Rect position, int elementIndex, bool isActive) {
      var element = prop.GetArrayElementAtIndex(elementIndex);
      position.height = GetElementHeight(element, elementIndex);
      DrawElement(position, element, elementIndex, isActive);
      position.y += position.height;
    }

    private void DrawElementBackgroundCallback(Rect position, int elementIndex, bool isActive, bool isFocused) {
      if (prop.isExpanded == false)
        return;

      var length = prop.arraySize;
      var element = default(SerializedProperty);

      var activeIndex = base.index;
      if (activeIndex == elementIndex && isActive == false) {
        // HACK: ReorderableList invokes this callback with the wrong elementIndex.
        if (nonDragTargetIndices != null) {
          elementIndex = nonDragTargetIndices[elementIndex];
        }
      }

      if (elementIndex >= 0 && elementIndex < length) {
        // HACK: ReorderableList invokes this callback with the wrong height.
        position.height = ElementHeightCallback(elementIndex);
        element = prop.GetArrayElementAtIndex(elementIndex);
      }

      DrawElementBackground(position, element, elementIndex, isActive, isFocused);

      if (element != null) {
        HandleElementEvents(position, elementIndex);
      }
    }

    private void DrawEmptyElementCallback(Rect position) {
      using (DisabledScope(v => true)) {
        EditorGUI.LabelField(position, "List is Empty");
      }
    }

    //======================================================================

    private void HandleElementEvents(Rect position, int elementIndex) {
      var current = Event.current;
      if (current == null) return;

      var handleRect = position;
      handleRect.width = 19;

      var isMouseInRect = (current.button == 0 || current.button == 1) && handleRect.Contains(current.mousePosition);

      var isActiveElementIndex = index == elementIndex;

      switch (current.type) {
        case EventType.MouseDown:
          if (isMouseInRect) {
            endEditingActiveTextField();
            index = elementIndex;
            return;
          }
          break;

        case EventType.MouseUp:
          if (isMouseInRect && isActiveElementIndex && current.button == 1) {
            DoElementContextMenu(handleRect, elementIndex);
            return;
          }
          break;
      }
    }

    //======================================================================

    private void DoElementContextMenu(Rect position, int elementIndex) {
      position.x += 1;
      position.height = elementHeight - 1;

      var menu = new GenericMenu();

      PopulateElementContextMenu(menu, elementIndex);

      if (menu.GetItemCount() > 0)
        menu.DropDown(position);
    }

    //======================================================================

    private static readonly FieldInfo m_NonDragTargetIndicesField = typeof(ReorderableList).GetField("m_NonDragTargetIndices", BindingFlags.Instance | BindingFlags.NonPublic);

    private List<int> nonDragTargetIndices {
      get => (List<int>)m_NonDragTargetIndicesField.GetValue(this);
    }

    //======================================================================

    protected virtual float extraSpacing => 0;

    private float AddElementPadding(float elementHeight) {
      return extraSpacing
        + spacing
        + elementHeight;
    }

    private void RemoveElementPadding(ref Rect position) {
      var verticalSpacing = EditorGUIUtility.standardVerticalSpacing;
      position.yMin += (extraSpacing + verticalSpacing) / 2;
      position.yMax -= (extraSpacing + verticalSpacing) / 2;
    }

    //======================================================================

    private delegate void EndEditingActiveTextFieldDelegate();

    private static readonly EndEditingActiveTextFieldDelegate endEditingActiveTextField =
      (EndEditingActiveTextFieldDelegate)Delegate.CreateDelegate(
        typeof(EndEditingActiveTextFieldDelegate),
        null,
        typeof(EditorGUI).GetMethod("EndEditingActiveTextField", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
      );

    //======================================================================

    protected static Deferred ColorAlphaScope(float a) {
      var oldColor = GUI.color;
      GUI.color = new Color(1, 1, 1, a);
      return new Deferred(() => GUI.color = oldColor);
    }

  }

}
#endif