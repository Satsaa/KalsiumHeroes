
namespace Muc.Data {

  using System;
  using System.Collections.Generic;
  using UnityEngine;
  using System.Linq;

  [Serializable]
  public class SerializedType : ISerializationCallbackReceiver {

    public static implicit operator Type(SerializedType t) => t?.type;

    [NonSerialized]
    protected bool updated = false;

    [SerializeField]
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
      if (_name == null) _type = null;
      else _type = Type.GetType(_name);
      updated = true;
    }

    public virtual IEnumerable<Type> GetValidTypes() {
      return AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(v => v.GetTypes())
        .Where(v => v.IsClass || v.IsInterface || v.IsValueType);
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize() { }
    void ISerializationCallbackReceiver.OnAfterDeserialize() => Update();
  }

  [Serializable]
  public class SerializedType<T> : SerializedType {

    public static implicit operator Type(SerializedType<T> t) => t?.type;

    protected override void Update() {
      var newtype = Type.GetType(_name ?? "");
      if (newtype != null) _type = typeof(T).IsAssignableFrom(newtype) ? newtype : null;
      updated = true;
    }

    public override IEnumerable<Type> GetValidTypes() {
      return AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(v => v.GetTypes())
        .Where(v =>
          (v.IsClass || v.IsInterface || v.IsValueType) &&
          (v == typeof(T) || typeof(T).IsAssignableFrom(v))
        );
    }
  }

}


#if UNITY_EDITOR
namespace Muc.Data {

  using System;
  using System.Linq;
  using System.Collections.Generic;
  using UnityEngine;
  using UnityEditor;
  using static Muc.Editor.PropertyUtil;
  using static Muc.Editor.EditorUtil;

  [CanEditMultipleObjects]
  [CustomPropertyDrawer(typeof(SerializedType), true)]
  public class SerializedTypeDrawer : PropertyDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

      var noLabel = label.text is "" && label.image is null;

      var values = GetValues<SerializedType>(property);
      var value = values.First();

      using (PropertyScope(position, label, property, out label)) {
        // Label
        if (!noLabel) {
          EditorGUI.LabelField(position, label);
          position.xMin += EditorGUIUtility.labelWidth + spacing;
        }
        // Dropdown
        var hint = new GUIContent(label); // Inherit state from label
        hint.text = value.type == null ? "null" : $"{value.type.ToString()} ({value.type.Assembly.GetName().Name})";
        if (EditorGUI.DropdownButton(position, new GUIContent(hint), FocusType.Keyboard)) {
          var types = value.GetValidTypes();
          var menu = BuildMenu(property, types.ToList(), values);
          menu.DropDown(position);
        }
      }
    }

    private static GenericMenu BuildMenu(SerializedProperty property, List<Type> types, IEnumerable<SerializedType> targets) {
      var menu = new GenericMenu();

      if (types.Count > 50) {

        // Ensure "No Namespace" comes first if it is used.
        var firstNoNamespace = types.FindIndex(v => v.Namespace == null && !v.FullName.Contains("<PrivateImplementationDetails>"));
        if (firstNoNamespace != -1) {
          var type = types[firstNoNamespace];
          var content = new GUIContent($"No Namespace/{type.ToString().Replace('.', '/')} ({type.Assembly.GetName().Name})");
          menu.AddItem(content, targets.Any(t => type == t.type), () => { OnSelect(property, type); });
        }

        for (int i = 0; i < types.Count; i++) {
          if (i == firstNoNamespace) continue;
          var type = types[i];
          if (type.FullName.Contains("<PrivateImplementationDetails>")) continue;
          UnityEngine.GUIContent content;
          if (type.Namespace == null) {
            content = new GUIContent($"No Namespace/{type.ToString().Replace('.', '/')} ({type.Assembly.GetName().Name})");
          } else {
            content = new GUIContent($"{type.ToString().Replace('.', '/')} ({type.Assembly.GetName().Name})");
          }
          menu.AddItem(content, targets.Any(t => type == t.type), () => { OnSelect(property, type); });
        }

      } else {

        foreach (var type in types) {
          var content = new GUIContent($"{type.ToString()} ({type.Assembly.GetName().Name})");
          menu.AddItem(content, targets.Any(t => type == t.type), () => { OnSelect(property, type); });
        }

      }

      return menu;
    }

    private static void OnSelect(SerializedProperty property, Type type) {
      var values = GetValues<SerializedType>(property);
      Undo.RecordObjects(property.serializedObject.targetObjects, $"Set {property.name}");
      foreach (var value in values) value.type = type;
      foreach (var target in property.serializedObject.targetObjects) EditorUtility.SetDirty(target);
      property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

  }
}
#endif
