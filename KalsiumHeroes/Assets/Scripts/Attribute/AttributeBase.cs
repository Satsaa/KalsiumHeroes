
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
public abstract class AttributeBase {

  internal static bool allow { get; private set; }
  internal static Dictionary<object, AttributeBase> keyTarget;

  internal static Deferred ConfigurationScope(Dictionary<object, AttributeBase> keyTarget) {
    allow = true;
    AttributeBase.keyTarget = keyTarget;
    return new Deferred(() => { allow = false; keyTarget = null; });
  }

  public abstract bool hasAlterers { get; }

  public static void RemoveAlterers() {
    foreach (var kv in keyTarget) {
      var k = kv.Key;
      var v = kv.Value;
      v.RemoveAlterer(k);
    }
    keyTarget.Clear();
  }

  /// <summary> Internal use only. Attribute alterers are removed automatically. </summary>
  public abstract void RemoveAlterer(object key);
}
