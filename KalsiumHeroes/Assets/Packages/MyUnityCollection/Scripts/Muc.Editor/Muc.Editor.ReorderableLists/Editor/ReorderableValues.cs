

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


  internal class ReorderableValues : ReorderableList {

    public readonly Type listType;
    public readonly Type elementType;
    public virtual bool showElementHeader => false;
    public readonly bool isReferenceList;
    public readonly SerializedProperty[] serializedProperties;
    protected static readonly new Defaults defaultBehaviours = new Defaults();
    protected readonly GUIContent titleContent = new GUIContent();

    //======================================================================

    public ReorderableValues(SerializedProperty primaryProperty, Type listType, Type elementType)
      : base(primaryProperty.serializedObject, primaryProperty.Copy(), true, true, true, true) {

      this.listType = listType;
      this.elementType = elementType;
      this.serializedProperties = AcquireSerializedProperties(this.serializedProperty);
      this.isReferenceList = primaryProperty.arrayElementType == "managedReference<>";
      footerHeight = 0;

      drawHeaderCallback = DrawHeaderCallback;
      drawFooterCallback = DrawFooterCallback;
      elementHeightCallback = ElementHeightCallback;
      drawElementCallback = DrawElementCallback;
      drawElementBackgroundCallback = DrawElementBackgroundCallback;
      drawNoneElementCallback = DrawEmptyElementCallback;

      onAddCallback = OnAddCallback;
      onCanRemoveCallback = OnCanRemoveCallback;
      onRemoveCallback = OnRemoveCallback;

      onSelectCallback = OnSelectCallback;
      onReorderCallback = OnReorderCallback;
    }

    //======================================================================

    private int dragIndex = 0;

    private void OnSelectCallback(ReorderableList list) {
      dragIndex = list.index;
    }

    private void OnReorderCallback(ReorderableList list) {
      if (dragIndex < 0) return;

      var dropIndex = list.index;
      if (dropIndex < 0) return;

      try {
        for (int i = 1; i < serializedProperties.Length; ++i) {
          var array = serializedProperties[i];
          array.MoveArrayElement(dragIndex, dropIndex);
        }
      } catch (Exception ex) {
        Debug.LogException(ex);
      }
      GUI.changed = true;
    }

    //======================================================================

    private static SerializedProperty[] AcquireSerializedProperties(SerializedProperty primaryProperty) {
      return new[] { primaryProperty };
    }

    //======================================================================

    public float GetHeight(GUIContent label) {
      UpdateLabel(label);
      if (!serializedProperty.isExpanded) return 20;
      UpdateElementHeights();
      var height = GetHeight();
      return height + (count < 1 ? -1 : -0);
    }

    public virtual void DoGUI(Rect position) {
      onNextGUIFrame?.Invoke();
      onNextGUIFrame = null;

      if (isReferenceList) displayAdd = this.count > 0;

      position = EditorGUI.IndentedRect(position);

      using (new EditorGUI.IndentLevelScope(-EditorGUI.indentLevel)) {
        if (serializedProperty.isExpanded) {

          if (count < 1) {
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
            if (drawNoneElementCallback == null)
              defaultBehaviours.DrawNoneElement(elementContentRect, draggable);
            else
              drawNoneElementCallback(elementContentRect);
          } else {
            DoList(position);
          }
        } else {
          index = -1;
          DoCollapsedListBackground(position);
        }

        DrawHeader(position);

        if (displayAdd || displayRemove) {
          var footerRect = position;
          footerRect.yMin += 2;
          DrawFooter(footerRect, this);
        }
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

    private void OnAddCallback(ReorderableList list) {
      serializedProperty.isExpanded = true;
      InsertElement(serializedProperty.arraySize);
    }

    private bool OnCanRemoveCallback(ReorderableList list) {
      return serializedProperty.isExpanded;
    }

    private void OnRemoveCallback(ReorderableList list) {
      DeleteElement(index);
    }

    //======================================================================

    [Serializable]
    private class ClipboardContent {
      public ClipboardElement[] elements;

      public ClipboardContent(int elementCount) {
        elements = new ClipboardElement[elementCount];
      }

      public static ClipboardContent Deserialize(string s) {
        try {
          return JsonUtility.FromJson<ClipboardContent>(s);
        } catch {
          return null;
        }
      }

      public string Serialize() {
        return JsonUtility.ToJson(this);
      }
    }

    [Serializable]
    private struct ClipboardElement {
      public string type;
      public string assemblyQualifiedName;
      public string json;
      public int instanceId;
    }

    private ClipboardContent CopyElementContent(int elementIndex) {
      if (elementIndex < 0) throw new IndexOutOfRangeException("Index must be non-negative.");

      var arrayIndex = 0;
      var arrayCount = serializedProperties.Length;
      var clipboardContent = new ClipboardContent(arrayCount);
      foreach (var array in serializedProperties) {
        var arrayObj = (IList)array.GetObject();
        var elementObj = arrayObj[elementIndex];
        var elementType = elementObj.GetType();
        string elementJson;
        if (elementType.IsPrimitive || elementType == typeof(string)) {
          elementJson = elementObj.ToString();
        } else {
          elementJson = JsonUtility.ToJson(elementObj);
        }
        var clipboardElement = new ClipboardElement();
        clipboardElement.type = elementType.FullName;
        clipboardElement.assemblyQualifiedName = elementType.AssemblyQualifiedName;
        clipboardElement.json = elementJson;
        if (elementObj is UnityEngine.Object asd) clipboardElement.instanceId = asd.GetInstanceID();
        clipboardContent.elements[arrayIndex] = clipboardElement;
        arrayIndex += 1;
      }
      return clipboardContent;
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

    private bool CanPaste(ClipboardContent clipboardContent) {
      if (clipboardContent == null) return false;

      var arrayIndex = 0;
      var arrayCount = serializedProperties.Length;
      foreach (var array in serializedProperties) {
        var arrayObj = (IList)array.GetObject();
        var arrayType = arrayObj.GetType();
        var elementType = arrayType.IsArray ? arrayType.GetElementType() : arrayType.GetGenericArguments()[0];
        var clipboardElement = clipboardContent.elements[arrayIndex++];
        var clipboardType = Type.GetType(clipboardElement.assemblyQualifiedName);
        if (!elementType.IsAssignableFrom(clipboardType)) return false;
      }
      return true;
    }

    private void PasteElement(int elementIndex, ClipboardContent clipboardContent) {
      if (elementIndex < 0) return;

      var clipboardElements = clipboardContent.elements;
      if (clipboardElements.Length == 0) return;

      var arrayIndex = 0;
      var arrayCount = serializedProperties.Length;
      var serializedProperty = this.serializedProperty;
      var serializedObject = serializedProperty.serializedObject;
      var targetObject = serializedObject.targetObject;
      Undo.RecordObject(targetObject, $"Paste {clipboardElements[0].type}");
      foreach (var array in serializedProperties) {
        if (elementIndex >= array.arraySize)
          array.arraySize = elementIndex + 1;

        var clipboardElement = clipboardContent.elements[arrayIndex++];
        var arrayObj = (IList)array.GetObject();
        var arrayType = array.GetObject().GetType();
        var elementObj = arrayObj[elementIndex];
        var elementType = arrayType.IsArray ? arrayType.GetElementType() : arrayType.GetGenericArguments()[0];
        var elementJson = clipboardElement.json;
        var elementInstanceId = clipboardElement.instanceId;

        if (elementType.IsPrimitive || elementType == typeof(string)) {
          object newValue;
          switch (elementObj) {
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
          arrayObj[elementIndex] = Convert.ChangeType(newValue, elementType);
        } else if (typeof(UnityEngine.Object).IsAssignableFrom(elementType)) {
          var fromId = EditorUtility.InstanceIDToObject(elementInstanceId);
          if (fromId != null) arrayObj[elementIndex] = fromId;
        } else {
          var fromJson = JsonUtility.FromJson(elementJson, elementType);
          if (fromJson != null) arrayObj[elementIndex] = fromJson;
        }
      }
      serializedObject.Update();
      GUI.changed = true;
    }

    //======================================================================

    protected static MemberInfo[] GetFirstMemberInHierarchy(Type type, string name, BindingFlags bindingAttr) {
      MemberInfo[] res;
      do {
        res = type.GetMember(name, bindingAttr);
        if (res.Length != 0) return res;
      } while ((type = type.BaseType) != null);
      return res;
    }

    private static MemberInfo[] GetFirstMemberInHierarchy(Type type, string name, MemberTypes memberTypes, BindingFlags bindingAttr) {
      MemberInfo[] res;
      do {
        res = type.GetMember(name, memberTypes, bindingAttr);
        if (res.Length != 0) return res;
      } while ((type = type.BaseType) != null);
      return res;
    }

    object GetMemberValue(object container, string name) {
      if (container == null) return null;
      var type = container.GetType();
      var members = GetFirstMemberInHierarchy(type, name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
      for (int i = 0; i < members.Length; ++i) {
        if (members[i] is FieldInfo field)
          return field.GetValue(container);
        else if (members[i] is PropertyInfo property)
          return property.GetValue(container);
      }
      return null;
    }

    void SetMemberValue(object container, string name, object value) {
      var type = container.GetType();
      var members = GetFirstMemberInHierarchy(type, name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
      for (int i = 0; i < members.Length; ++i) {
        if (members[i] is FieldInfo field) {
          field.SetValue(container, value);
          return;
        } else if (members[i] is PropertyInfo property) {
          property.SetValue(container, value);
          return;
        }
      }
    }

    object GetPathComponentValue(object container, PropertyPathComponent component) {
      if (component.propertyName == null)
        return ((IList)container)[component.elementIndex];
      else
        return GetMemberValue(container, component.propertyName);
    }

    void SetPathComponentValue(object container, PropertyPathComponent component, object value) {
      if (component.propertyName == null)
        ((IList)container)[component.elementIndex] = value;
      else
        SetMemberValue(container, component.propertyName, value);
    }

    Regex arrayElementRegex = new Regex(@"\GArray\.data\[(\d+)\]", RegexOptions.Compiled);

    struct PropertyPathComponent {
      public string propertyName;
      public int elementIndex;
    }

    bool NextPathComponent(string propertyPath, ref int index, out PropertyPathComponent component) {
      component = new PropertyPathComponent();

      if (index >= propertyPath.Length) return false;

      var arrayElementMatch = arrayElementRegex.Match(propertyPath, index);
      if (arrayElementMatch.Success) {
        index += arrayElementMatch.Length + 1; // Skip past next '.'
        component.elementIndex = int.Parse(arrayElementMatch.Groups[1].Value);
        return true;
      }

      int dot = propertyPath.IndexOf('.', index);
      if (dot == -1) {
        component.propertyName = propertyPath.Substring(index);
        index = propertyPath.Length;
      } else {
        component.propertyName = propertyPath.Substring(index, dot - index);
        index = dot + 1; // Skip past next '.'
      }

      return true;
    }

    public void SetValueNoRecord(SerializedProperty property, object value) {
      string propertyPath = property.propertyPath;
      object container = property.serializedObject.targetObject;

      int i = 0;
      NextPathComponent(propertyPath, ref i, out var deferredToken);
      while (NextPathComponent(propertyPath, ref i, out var token)) {
        container = GetPathComponentValue(container, deferredToken);
        deferredToken = token;
      }
      Debug.Assert(!container.GetType().IsValueType, $"Cannot use SerializedObject.SetValue on a struct object, as the result will be set on a temporary. Either change {container.GetType().Name} to a class, or use SetValue with a parent member.");
      SetPathComponentValue(container, deferredToken, value);
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
      if (elementIndex < 0) return;

      var serializedProperty = this.serializedProperty;
      var serializedObject = serializedProperty.serializedObject;
      foreach (var array in serializedProperties) {
        array.InsertArrayElementAtIndex(elementIndex);

        var type = array.arrayElementType;

        // Create first element with default values and 
        if (array.arraySize == 1) {
          var element = array.GetArrayElementAtIndex(elementIndex);
          var elPropType = element.propertyType;
          if (isReferenceList) {
            if (element.managedReferenceFieldTypename != "mscorlib System.Object") {

            }
          } else {

            switch (elPropType) {

              default:
                break;

              case SerializedPropertyType.Generic:

                var elementType = GetSerializedPropertyType(element);

                if (elementType != null) {
                  object instance = null;
                  try {
                    instance = Activator.CreateInstance(elementType, true);
                  } catch (Exception) {
                    instance = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(elementType);
                  } finally {
                    if (instance != null) {
                      serializedObject.ApplyModifiedProperties();
                      SetValueNoRecord(element, instance);
                    }
                  }
                }
                break;
            }
          }
        } else {
          if (isReferenceList) {
            var element = array.GetArrayElementAtIndex(elementIndex);

            var copyIndex = elementIndex == 0 ? 1 : elementIndex - 1;
            var copyElement = array.GetArrayElementAtIndex(copyIndex);
            var copyElementType = GetManagedReferenceType(copyElement);

            object instance = null;
            try {
              instance = Activator.CreateInstance(copyElementType, true);
            } catch (Exception) {
              instance = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(copyElementType);
            } finally {
              if (instance != null) {
                serializedObject.ApplyModifiedProperties();
                element.managedReferenceValue = instance;
                //SetValueNoRecord(element, instance);
              }
            }
          }
        }

      }
      serializedObject.ApplyModifiedProperties();
      index = elementIndex;
      GUI.changed = true;
    }

    public static Type GetSerializedPropertyType(SerializedProperty property) {
      var parentType = property.serializedObject.targetObject.GetType();
      return GetTypeByPath(parentType, property.propertyPath);
    }

    public static Type GetTypeByPath(Type type, string path) {
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
      if (elementIndex < 0) return;

      var serializedProperty = this.serializedProperty;
      var serializedObject = serializedProperty.serializedObject;
      if (elementIndex < serializedProperty.arraySize) {
        foreach (var array in serializedProperties) {
          var element = array.GetArrayElementAtIndex(elementIndex);
          var oldSubassets = element.FindReferencedSubassets();
          var preDelSize = array.arraySize;
          array.DeleteArrayElementAtIndex(elementIndex);
          if (preDelSize == array.arraySize)
            array.DeleteArrayElementAtIndex(elementIndex);
          if (oldSubassets.Any()) {
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            serializedObject.DestroyUnreferencedSubassets(oldSubassets);
          } else {
            serializedObject.ApplyModifiedProperties();
          }
        }

        var length = serializedProperty.arraySize;
        if (index > length - 1)
          index = length - 1;
      }
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

    protected static readonly GUIStyle elementBackgroundStyle = "RL Background";

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
      var serializedProperty = this.serializedProperty;
      var serializedObject = serializedProperty.serializedObject;

      menu.AddItem(cutLabel, false, () => OnNextGUIFrame(() => CutElement(elementIndex)));
      menu.AddItem(copyLabel, false, () => CopyElementToClipboard(elementIndex));
      var content = ClipboardContent.Deserialize(EditorGUIUtility.systemCopyBuffer);
      var canPaste = CanPaste(content);
      if (canPaste) menu.AddItem(pasteLabel, false, () => OnNextGUIFrame(() => PasteElement(elementIndex, content)));
      else menu.AddDisabledItem(pasteLabel);

      if (displayRemove) {
        menu.AddItem(deleteLabel, false, () => OnNextGUIFrame(() => DeleteElement(elementIndex)));
      }
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

    private void DrawHeader(Rect position) {
      defaultBehaviours.DrawHeaderBackground(position);
      position.xMin += 15;
      position.y++;
      position.height = EditorGUIUtility.singleLineHeight;

      var foldoutRect = position;
      foldoutRect.width -= 50;
      var property = serializedProperty;
      var wasExpanded = property.isExpanded;
      var isExpanded = EditorGUI.Foldout(foldoutRect, wasExpanded, label, true);
      if (isExpanded != wasExpanded) {
        property.isExpanded = isExpanded;
      }
    }

    //======================================================================

    private static GUIContent iconToolbarPlus = EditorGUIUtility.TrIconContent("Toolbar Plus", "Add to list");
    private static GUIContent iconToolbarPlusMore = EditorGUIUtility.TrIconContent("Toolbar Plus More", "Choose to add to list");
    private static GUIContent iconToolbarMinus = EditorGUIUtility.TrIconContent("Toolbar Minus", "Remove selection or last element from list");
    private readonly GUIStyle preButton = "RL FooterButton";
    private readonly GUIStyle footerBackground = "RL Footer";

    // draw the default footer
    public void DrawFooter(Rect rect, ReorderableList list) {
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
        using (new EditorGUI.DisabledScope(
            list.onCanAddCallback != null && !list.onCanAddCallback(list))) {
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
        using (
          new EditorGUI.DisabledScope(
            list.index < 0 || list.index >= list.count ||
            (list.onCanRemoveCallback != null && !list.onCanRemoveCallback(list))
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

    internal void UpdateLabel(GUIContent label) {
      this.label.image = label.image;

      var tooltip = label.tooltip;
      if (string.IsNullOrEmpty(tooltip)) {
        tooltip = serializedProperty.tooltip;
      }
      this.label.tooltip = tooltip;

      var arraySize = serializedProperty.arraySize;

      var text = label.text ?? string.Empty;
      text = $"{text} ({arraySize})";
      this.label.text = text;
    }

    //======================================================================

    private readonly List<float> elementHeights = new List<float>();

    private void UpdateElementHeights() {
      var primaryProperty = serializedProperty;
      var elementCount = primaryProperty.arraySize;
      elementHeights.Clear();
      elementHeights.Capacity = elementCount;
      for (int i = 0; i < elementCount; ++i)
        elementHeights.Add(0);

      if (primaryProperty.isExpanded) {
        var spacing = EditorGUIUtility.standardVerticalSpacing;
        var arrayCount = 0;
        foreach (var array in serializedProperties) {
          for (int i = 0; i < elementCount; ++i) {

            var element = array.GetArrayElementAtIndex(i);
            var elementHeight = GetElementHeight(element, i);
            if (arrayCount > 0)
              elementHeight += spacing;
            elementHeights[i] += elementHeight;
          }
          arrayCount += 1;
        }
        for (int i = 0; i < elementCount; ++i) {
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
      var primaryProperty = serializedProperty;
      if (primaryProperty.isExpanded) {
        RemoveElementPadding(ref position);
        DrawElementRows(position, elementIndex, isActive);
      }
    }

    private void DrawElementRows(Rect position, int elementIndex, bool isActive) {
      var spacing = EditorGUIUtility.standardVerticalSpacing;
      var loopCounter = 0;
      foreach (var array in serializedProperties) {
        if (loopCounter++ > 0)
          position.y += spacing;

        var element = array.GetArrayElementAtIndex(elementIndex);
        position.height = GetElementHeight(element, elementIndex);
        DrawElement(position, element, elementIndex, isActive);
        position.y += position.height;
      }
    }

    private void DrawElementBackgroundCallback(Rect position, int elementIndex, bool isActive, bool isFocused) {
      var array = this.serializedProperty;
      if (array.isExpanded == false)
        return;

      var length = array.arraySize;
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
        element = array.GetArrayElementAtIndex(elementIndex);
      }

      DrawElementBackground(position, element, elementIndex, isActive, isFocused);

      if (element != null) {
        HandleElementEvents(position, elementIndex);
      }
    }

    private void DrawEmptyElementCallback(Rect position) {
      using (EditorUtil.DisabledScope(v => true)) {
        EditorGUI.LabelField(position, "List is Empty");
      }
    }

    //======================================================================

    private void HandleElementEvents(Rect position, int elementIndex) {
      var current = Event.current;
      if (current == null) return;

      var handleRect = position;
      var menuRect = Rect.zero;
      if (showElementHeader) {
        handleRect.width += 1;
        menuRect = position;
        menuRect.xMin = menuRect.xMax - 16;
      } else {
        handleRect.width = 19;
      }

      var isLeftMouseInMenuRect = current.button == 0 && menuRect.Contains(current.mousePosition);

      var isRightMouseInHandleRect = current.button == 1 && handleRect.Contains(current.mousePosition);

      var isMouseInRect = isLeftMouseInMenuRect || isRightMouseInHandleRect;

      var isActiveElementIndex = index == elementIndex;

      switch (current.type) {
        case EventType.MouseDown:
          if (isMouseInRect) {
            EndEditingActiveTextField();
            index = elementIndex;
            return;
          }
          break;

        case EventType.MouseUp:
          if (isMouseInRect && isActiveElementIndex) {
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
        + EditorUtil.spacing
        + elementHeight;
    }

    private void RemoveElementPadding(ref Rect position) {
      var verticalSpacing = EditorGUIUtility.standardVerticalSpacing;
      position.yMin += (extraSpacing + verticalSpacing) / 2;
      position.yMax -= (extraSpacing + verticalSpacing) / 2;
    }

    //======================================================================

    private delegate void EndEditingActiveTextFieldDelegate();

    private static readonly EndEditingActiveTextFieldDelegate EndEditingActiveTextField =
      (EndEditingActiveTextFieldDelegate)Delegate.CreateDelegate(
        typeof(EndEditingActiveTextFieldDelegate),
        null,
        typeof(EditorGUI).GetMethod("EndEditingActiveTextField", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
      );

    //======================================================================

    protected static Deferred BackgroundColorScope(Color newColor) {
      var oldColor = GUI.backgroundColor;
      GUI.backgroundColor = newColor;
      return new Deferred(() => GUI.backgroundColor = oldColor);
    }

    protected static Deferred ColorScope(Color newColor) {
      var oldColor = GUI.color;
      GUI.color = newColor;
      return new Deferred(() => GUI.color = oldColor);
    }

    protected static Deferred ColorAlphaScope(float a) {
      var oldColor = GUI.color;
      GUI.color = new Color(1, 1, 1, a);
      return new Deferred(() => GUI.color = oldColor);
    }

  }

}
#endif