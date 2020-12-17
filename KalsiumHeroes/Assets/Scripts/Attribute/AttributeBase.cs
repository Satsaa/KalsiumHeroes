
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

	public enum AttributeProperty {
		Primary,
		Secondary,
		Enabled,
	}

	public abstract bool HasAlteredValue(AttributeProperty attributeProperty);
	public abstract string GetEditorLabel(AttributeProperty attributeProperty);
	public virtual bool DisplayAlteredInPlay(AttributeProperty attributeProperty) => false;

}
