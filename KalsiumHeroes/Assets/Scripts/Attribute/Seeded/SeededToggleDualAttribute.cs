
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
public class SeededToggleDualAttribute<T> : SeededDualAttribute<T> {

  [Tooltip("Attribute is enabled?")]
  public bool enabled;

  protected Dictionary<object, Func<bool, bool>> enabledAlterers = new Dictionary<object, Func<bool, bool>>();


  public SeededToggleDualAttribute(bool enabled = true) {
    this.enabled = enabled;
  }
  public SeededToggleDualAttribute(T value, T other, bool enabled = true) : base(value, other) {
    this.enabled = enabled;
  }


  /// <summary> Registers a function that alters what the other property returns. </summary>
  public void RegisterSecondaryAlterer(Func<bool, bool> alterer) {
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
[CustomPropertyDrawer(typeof(SeededToggleDualAttribute<>))]
public class SeededToggleDualAttributeDrawer : ToggleDualAttributeDrawer {

}
#endif