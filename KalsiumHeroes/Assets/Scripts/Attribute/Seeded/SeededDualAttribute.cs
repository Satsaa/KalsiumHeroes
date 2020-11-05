
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
public class SeededDualAttribute<T> : SeededAttribute<T> {

  [SerializeField]
  [FormerlySerializedAs(nameof(other))]
  [Tooltip("Secondary value")]
  protected T _other;

  public virtual T other {
    get => otherAlterers.Values.Aggregate(_other, (current, alt) => alt(current));
  }

  protected Dictionary<object, Func<T, T>> otherAlterers = new Dictionary<object, Func<T, T>>();


  public SeededDualAttribute() { }
  public SeededDualAttribute(T value, T other) : base(value) {
    this._other = other;
  }

  /// <summary> Registers a function that alters what the other property returns. </summary>
  public void RegisterSecondaryAlterer(Func<T, T> alterer) {
    if (!AttributeBase.allow) throw new SecurityException("Configuring alterers is only allowed inside the RegisterAttributeAlterers function!");
    var keyObject = new object();
    otherAlterers.Add(keyObject, alterer);
    AttributeBase.keyTarget.Add(keyObject, this);
  }

  /// <summary> Internal use only. Attribute alterers are removed automatically. </summary>
  public override void RemoveAlterer(object key) {
    if (!AttributeBase.allow) throw new SecurityException("Configuring alterers is only allowed inside the RegisterAttributeAlterers function!");
    alterers.Remove(key);
    otherAlterers.Remove(key);
  }

}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SeededDualAttribute<>))]
public class SeededDualAttributeDrawer : DualAttributeDrawer {

}
#endif