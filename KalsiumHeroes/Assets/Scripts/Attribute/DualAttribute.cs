
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
public class DualAttribute<T> : Attribute<T> {

  [SerializeField]
  [FormerlySerializedAs(nameof(other))]
  [Tooltip("Secondary value")]
  protected T _other;

  public virtual T other {
    get => otherAlterers.Values.Aggregate(_other, (current, alt) => alt(current));
    set => _other = value;
  }

  protected Dictionary<object, Func<T, T>> otherAlterers = new Dictionary<object, Func<T, T>>();
  public override bool hasAlterers => alterers.Count > 0 || otherAlterers.Count > 0;

  public DualAttribute() { }
  public DualAttribute(T value, T other) : base(value) {
    _other = other;
  }


  /// <summary> Registers a function that alters what the other property returns. </summary>
  public void RegisterSecondaryAlterer(Func<T, T> alterer) {
    if (!allow) throw new SecurityException("Configuring alterers is only allowed inside the RegisterAttributeAlterers function!");
    var keyObject = new object();
    otherAlterers.Add(keyObject, alterer);
    keyTarget.Add(keyObject, this);
  }

  /// <summary> Internal use only. Attribute alterers are removed automatically. </summary>
  public override void RemoveAlterer(object key) {
    if (!allow) throw new SecurityException("Configuring alterers is only allowed inside the RegisterAttributeAlterers function!");
    alterers.Remove(key);
    otherAlterers.Remove(key);
  }

  /// <summary> Sets value to other. </summary>
  public void ResetValue() {
    value = other;
  }

  /// <summary> If value is larger than other, sets value to other. T must be castable to IComparable. </summary>
  public void LimitValue() {
    if (((IComparable)value).CompareTo((IComparable)other) > 0) {
      value = other;
    }
  }

}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(DualAttribute<>))]
public class DualAttributeDrawer : PropertyDrawer {

  public float alteredWidth => expandAltered ? 0.5f : 0.8f;
  public bool expandAltered = false;

  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

    using (PropertyScope(position, label, property, out label))
    using (RestoreLabelWidthScope())
    using (RestoreFieldWidthScope()) {

      var valueProperty = property.FindPropertyRelative("_value");
      var otherProperty = property.FindPropertyRelative("_other");

      var fieldInfo = GetFieldInfo(property);
      var labelAttribute = fieldInfo?.GetCustomAttributes(typeof(AttributeLabelsAttribute), false).FirstOrDefault() as AttributeLabelsAttribute;

      var noLabel = label.text is "" && label.image is null;
      var pos = position;
      pos.width = noLabel ? 0 : labelWidth;
      if (!noLabel) EditorGUI.LabelField(pos, label);

      using (IndentScope(v => 0)) {

        var obj = GetValues<AttributeBase>(property).First();
        var isRepaint = Event.current.type == EventType.Repaint;
        var prevExpandAltered = expandAltered;
        var expand = false;

        var valueLabel = new GUIContent(labelAttribute?.primaryLabel ?? "Value");
        labelWidth = GUI.skin.label.CalcSize(valueLabel).x - spacing;
        pos.xMin = pos.xMax + spacing;
        pos.width = (position.xMax - pos.xMin) / 2 - spacing;
        var origMax = pos.xMax;
        if (obj.hasAlterers) pos.width *= alteredWidth;
        EditorGUI.PropertyField(pos, valueProperty, valueLabel);

        if (obj.hasAlterers) {
          using (DisabledScope()) {
            pos.xMin = pos.xMax;
            pos.xMax = origMax;
            var prop = obj.GetType().GetProperty(nameof(DualAttribute<int>.value));
            var val = prop.GetValue(obj);
            EditorGUI.TextField(pos, val.ToString());
            if (isRepaint) {
              expand |= pos.Contains(Event.current.mousePosition);
              GUI.changed |= prevExpandAltered != expand;
            }
          }
        }

        var baseLabel = new GUIContent(labelAttribute?.secondaryLabel ?? "Other");
        labelWidth = GUI.skin.label.CalcSize(baseLabel).x - spacing;
        pos.xMin = pos.xMax + spacing;
        pos.xMax = position.xMax;
        origMax = pos.xMax;
        if (obj.hasAlterers) pos.width *= alteredWidth;
        EditorGUI.PropertyField(pos, otherProperty, baseLabel);

        if (obj.hasAlterers) {
          using (DisabledScope()) {
            pos.xMin = pos.xMax;
            pos.xMax = origMax;
            var prop = obj.GetType().GetProperty(nameof(DualAttribute<int>.other));
            var val = prop.GetValue(obj);
            EditorGUI.TextField(pos, val.ToString());
            if (isRepaint) {
              expand |= pos.Contains(Event.current.mousePosition);
              GUI.changed |= prevExpandAltered != expand;
            }
          }
        }

        if (isRepaint) {
          expandAltered = expand;
        }
      }

    }

  }

}
#endif