
#if UNITY_EDITOR
using UnityEditor;
using static Muc.Editor.EditorUtil;
using static Muc.Editor.PropertyUtil;
#endif

using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Serialization;
using System;
using System.Security;
using Muc;

[System.Serializable]
public class Attribute<T> : AttributeBase {

  [SerializeField]
  [FormerlySerializedAs(nameof(value))]
  [Tooltip("Primary value")]
  protected T _value;

  public virtual T value {
    get => alterers.Values.Aggregate(_value, (current, alt) => alt(current));
    set => _value = value;
  }

  protected Dictionary<object, Func<T, T>> alterers = new Dictionary<object, Func<T, T>>();
  public override bool hasAlterers => alterers.Count > 0;


  public Attribute() { }
  public Attribute(T value) {
    this._value = value;
  }


  /// <summary> Registers a function that alters what the value property returns. </summary>
  public void RegisterAlterer(Func<T, T> alterer) {
    if (!allow) throw new SecurityException("Configuring alterers is only allowed inside the RegisterAttributeAlterers function!");
    var keyObject = new object();
    alterers.Add(keyObject, alterer);
    keyTarget.Add(keyObject, this);
  }

  /// <summary> Internal use only. Attribute alterers are removed automatically. </summary>
  public override void RemoveAlterer(object key) {
    if (!allow) throw new SecurityException("Configuring alterers is only allowed inside the RegisterAttributeAlterers function!");
    alterers.Remove(key);
  }

}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Attribute<>))]
public class AttributeDrawer : PropertyDrawer {

  public float alteredWidth => expandAltered ? 0.5f : 0.8f;
  public bool expandAltered = false;

  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

    using (PropertyScope(position, label, property, out label))
    using (RestoreLabelWidthScope())
    using (RestoreFieldWidthScope()) {

      var valueProperty = property.FindPropertyRelative("_value");

      var fieldInfo = GetFieldInfo(property);
      var labelAttribute = fieldInfo?.GetCustomAttributes(typeof(AttributeLabelsAttribute), false).FirstOrDefault() as AttributeLabelsAttribute;

      var noLabel = label.text is "" && label.image is null;
      var pos = position;
      pos.width = noLabel ? 0 : labelWidth;
      if (!noLabel) EditorGUI.LabelField(pos, label);

      using (IndentScope(v => 0)) {

        var obj = GetValues<AttributeBase>(property).First();

        var valueLabel = new GUIContent(labelAttribute?.primaryLabel ?? "");
        labelWidth = 35;
        pos.xMin = pos.xMax + spacing;
        pos.xMax = position.xMax;
        if (obj.hasAlterers) pos.width *= alteredWidth;
        EditorGUI.PropertyField(pos, valueProperty, valueLabel);

        if (obj.hasAlterers) {
          using (DisabledScope()) {
            pos.xMin = pos.xMax;
            pos.xMax = position.xMax;
            var prop = obj.GetType().GetProperty(nameof(Attribute<int>.value));
            var val = prop.GetValue(obj);
            EditorGUI.TextField(pos, val.ToString());
            if (Event.current.type == EventType.Repaint) {
              var prev = expandAltered;
              expandAltered = pos.Contains(Event.current.mousePosition);
              GUI.changed |= prev != expandAltered;
            }
          }
        }
      }
    }

  }

}
#endif
