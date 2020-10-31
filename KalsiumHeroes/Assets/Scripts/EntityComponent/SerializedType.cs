
#if UNITY_EDITOR
using UnityEditor;
using static Muc.Editor.PropertyUtil;
using static Muc.Editor.EditorUtil;
#endif

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;

[Serializable]
public class SerializedType {

  public static implicit operator Type(SerializedType t) => t.type;

  [NonSerialized]
  protected bool updated = false;

  [HideInInspector, SerializeField]
  protected string _name;
  public string name {
    get {
      return _name;
    }
    set {
      _name = value;
      Update();
    }
  }

  protected Type _type;
  public Type type {
    get {
      if (!updated) Update();
      return _type;
    }
    set {
      _type = value;
      _name = _type?.AssemblyQualifiedName;
    }
  }

  protected virtual void Update() {
    _type = Type.GetType(_name);
    updated = true;
  }

  public virtual IEnumerable<Type> GetValidTypes() {
    return AppDomain.CurrentDomain.GetAssemblies()
      .SelectMany(v => v.GetTypes())
      .Where(v => v.IsClass && !v.IsAbstract && !v.ContainsGenericParameters);
  }

}


[Serializable]
public class SerializedType<T> : SerializedType {

  public static implicit operator Type(SerializedType<T> t) => t.type;

  protected override void Update() {
    var newtype = Type.GetType(_name);
    if (newtype != null) _type = newtype.IsSubclassOf(typeof(T)) ? newtype : null;
    updated = true;
  }

  public override IEnumerable<Type> GetValidTypes() {
    var type = typeof(T);
    return AppDomain.CurrentDomain.GetAssemblies()
      .SelectMany(v => v.GetTypes())
      .Where(v => v.IsClass && !v.IsAbstract && !v.ContainsGenericParameters)
      .Where(v => type == v || v.IsSubclassOf(type));
  }
}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SerializedType), true)]
public class SerializedTypeDrawer : PropertyDrawer {

  private static Dictionary<object, CacheData> typeCache = new Dictionary<object, CacheData>();
  private static Regex filter = new Regex(", Version.*");
  private int selected = -1;

  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

    var multi = property.serializedObject.targetObjects.Length > 1;

    if (multi) {
      var nameProperty = property.FindPropertyRelative("_name");
      using (DisabledScope(v => true))
      using (PropertyScope(position, label, property, out label)) {
        EditorGUI.TextField(position, label, nameProperty.stringValue);
      }
      return;
    }

    var value = (SerializedType)GetValue(property);

    GUIContent[] options = new GUIContent[0];
    if (!typeCache.TryGetValue(value, out var item) || item.types == null || item.displays == null) {
      var types = value.GetValidTypes();
      item = typeCache[value] = new CacheData();
      item.types = types.ToArray();
      item.displays = types.Select(v => new GUIContent(filter.Replace(v.AssemblyQualifiedName, ""))).ToArray();
    }
    options = item.displays;

    using (PropertyScope(position, label, property, out label)) {

      if (selected == -1 && value.name != null) {
        selected = Array.FindIndex(item.types, v => v == value.type);
        if (selected == -1) {
          var smallName = filter.Replace(value.name, "");
          selected = Array.FindIndex(item.displays, v => v.text == smallName);
        }
      }

      var prevSelected = selected;
      selected = EditorGUI.Popup(position, label, selected, options);
      if (prevSelected != selected) {
        value.type = item.types[selected];
        EditorUtility.SetDirty(property.serializedObject.targetObject);
        property.serializedObject.ApplyModifiedProperties();
      }

    }

  }

  class CacheData {
    public Type[] types;
    public GUIContent[] displays;
  }

}
#endif
