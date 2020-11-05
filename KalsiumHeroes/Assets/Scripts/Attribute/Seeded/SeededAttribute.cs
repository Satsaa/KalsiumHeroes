
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


[Serializable]
// Just 
public class SeededAttribute<T> : AttributeBase {

  [SerializeField]
  [FormerlySerializedAs(nameof(value))]
  [Tooltip("Seed value")]
  protected T _value;

  public virtual T value {
    get => alterers.Values.Aggregate(_value, (current, alt) => alt(current));
  }

  protected Dictionary<object, Func<T, T>> alterers = new Dictionary<object, Func<T, T>>();


  public SeededAttribute() { }
  public SeededAttribute(T value) {
    this._value = value;
  }


  /// <summary> Registers a function that alters what the value property returns. </summary>
  public void RegisterAlterer(Func<T, T> alterer) {
    if (!AttributeBase.allow) throw new SecurityException("Configuring alterers is only allowed inside the RegisterAttributeAlterers function!");
    var keyObject = new object();
    alterers.Add(keyObject, alterer);
    AttributeBase.keyTarget.Add(keyObject, this);
  }

  /// <summary> Internal use only. Attribute alterers are removed automatically. </summary>
  public override void RemoveAlterer(object key) {
    if (!AttributeBase.allow) throw new SecurityException("Configuring alterers is only allowed inside the RegisterAttributeAlterers function!");
    alterers.Remove(key);
  }

}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SeededAttribute<>))]
public class SeededAttributeDrawer : PropertyDrawer {

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
        labelWidth = 35;
        pos.xMin = pos.xMax + spacing;
        pos.xMax = position.xMax;
        using (DisabledScope(v => Application.isPlaying))
          EditorGUI.PropertyField(pos, valueProperty, new GUIContent(labelAttribute?.primaryLabel ?? ""));
      }

    }

  }

}
#endif
