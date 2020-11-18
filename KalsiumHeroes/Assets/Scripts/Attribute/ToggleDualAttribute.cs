
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
public class ToggleDualAttribute<T> : DualAttribute<T> {

  [SerializeField]
  [FormerlySerializedAs(nameof(enabled))]
  [Tooltip("Attribute is enabled?")]
  private bool _enabled;

  public virtual bool enabled {
    get => enabledAlterers.Values.Aggregate(_enabled, (current, alt) => alt(current));
    set => _enabled = value;
  }

  protected Dictionary<object, Func<bool, bool>> enabledAlterers = new Dictionary<object, Func<bool, bool>>();
  public override bool hasAlterers => alterers.Count > 0 || otherAlterers.Count > 0 || enabledAlterers.Count > 0;


  public ToggleDualAttribute(bool enabled = true) {
    _enabled = enabled;
  }
  public ToggleDualAttribute(T value, T other, bool enabled = true) : base(value, other) {
    _enabled = enabled;
  }


  internal void RegisterEnabledAlterer(Func<bool, bool> alterer) {
    if (!AttributeBase.allow) throw new SecurityException("Configuring alterers is only allowed inside the RegisterAttributeAlterers function!");
    var keyObject = new object();
    enabledAlterers.Add(keyObject, alterer);
    AttributeBase.keyTarget.Add(keyObject, this);
  }

  /// <summary> Internal use only. Attribute alterers are removed automatically. </summary>
  public override void RemoveAlterer(object key) {
    if (!AttributeBase.allow) throw new SecurityException("Configuring alterers is only allowed inside the RegisterAttributeAlterers function!");
    alterers.Remove(key);
    otherAlterers.Remove(key);
    enabledAlterers.Remove(key);
  }
}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ToggleDualAttribute<>))]
public class ToggleDualAttributeDrawer : PropertyDrawer {

  public float alteredWidth => expandAltered ? 0.5f : 0.8f;
  public bool expandAltered = false;

  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

    using (PropertyScope(position, label, property, out label))
    using (RestoreLabelWidthScope())
    using (RestoreFieldWidthScope()) {

      var enabledProperty = property.FindPropertyRelative("_enabled");
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

        pos.xMin = pos.xMax + spacing;
        pos.width = 15 + spacing;
        EditorGUI.PropertyField(pos, enabledProperty, GUIContent.none);

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
            var prop = obj.GetType().GetProperty(nameof(ToggleDualAttribute<int>.enabled));
            var val = prop.GetValue(obj);
            EditorGUI.TextField(pos, new GUIContent("", "Altered value"), val.ToString());
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
            var prop = obj.GetType().GetProperty(nameof(ToggleDualAttribute<int>.value));
            var val = prop.GetValue(obj);
            EditorGUI.TextField(pos, new GUIContent("", "Altered value"), val.ToString());
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