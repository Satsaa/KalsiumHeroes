
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
public class SeededToggleAttribute<T> : SeededAttribute<T> {

  [SerializeField]
  [FormerlySerializedAs(nameof(enabled))]
  [Tooltip("Attribute is enabled?")]
  private bool _enabled;

  public virtual bool enabled {
    get => enabledAlterers.Values.Aggregate(_enabled, (current, alt) => alt(current));
  }

  protected Dictionary<object, Func<bool, bool>> enabledAlterers = new Dictionary<object, Func<bool, bool>>();
  public override bool hasAlterers => alterers.Count > 0 || enabledAlterers.Count > 0;


  public SeededToggleAttribute(bool enabled = true) {
    _enabled = enabled;
  }
  public SeededToggleAttribute(T value, bool enabled = true) : base(value) {
    _enabled = enabled;
  }


  /// <summary> Registers a function that alters what the other property returns. </summary>
  public void RegisterSecondaryAlterer(Func<bool, bool> alterer) {
    if (!allow) throw new SecurityException("Configuring alterers is only allowed inside the RegisterAttributeAlterers function!");
    var keyObject = new object();
    enabledAlterers.Add(keyObject, alterer);
    keyTarget.Add(keyObject, this);
  }

  /// <summary> Internal use only. Attribute alterers are removed automatically. </summary>
  public override void RemoveAlterer(object key) {
    if (!allow) throw new SecurityException("Configuring alterers is only allowed inside the RegisterAttributeAlterers function!");
    alterers.Remove(key);
    enabledAlterers.Remove(key);
  }

}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SeededToggleAttribute<>))]
public class SeededToggleAttribute : PropertyDrawer {

  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

    using (PropertyScope(position, label, property, out label))
    using (RestoreLabelWidthScope())
    using (RestoreFieldWidthScope()) {

      var enabledProperty = property.FindPropertyRelative("_enabled");
      var valueProperty = property.FindPropertyRelative("_value");

      var fieldInfo = GetFieldInfo(property);
      var labelAttribute = fieldInfo?.GetCustomAttributes(typeof(AttributeLabelsAttribute), false).FirstOrDefault() as AttributeLabelsAttribute;

      var noLabel = label.text is "" && label.image is null;
      var pos = position;
      pos.width = noLabel ? 0 : labelWidth;
      if (!noLabel) EditorGUI.LabelField(pos, label);

      using (IndentScope(v => 0)) {

        var obj = GetValues<AttributeBase>(property).First();

        pos.xMin = pos.xMax + spacing;
        pos.width = 15 + spacing;
        EditorGUI.PropertyField(pos, enabledProperty, GUIContent.none);
        if (obj.hasAlterers) {
          using (DisabledScope()) {
            pos.x = pos.xMax;
            var prop = obj.GetType().GetProperty(nameof(ToggleAttribute<int>.enabled));
            var val = (bool)prop.GetValue(obj);
            EditorGUI.Toggle(pos, val);
          }
        }

        var valueLabel = new GUIContent(labelAttribute?.primaryLabel ?? "");
        pos.xMin = pos.xMax + spacing;
        pos.xMax = position.xMax;
        if (Application.isPlaying) {
          using (DisabledScope()) {
            pos.xMin = pos.xMax;
            pos.xMax = position.xMax;
            var prop = obj.GetType().GetProperty(nameof(SeededToggleDualAttribute<int>.value));
            var val = prop.GetValue(obj);
            EditorGUI.TextField(pos, valueLabel, val.ToString());
          }
        } else {
          EditorGUI.PropertyField(pos, valueProperty, valueLabel);
        }
      }

    }

  }

}
#endif