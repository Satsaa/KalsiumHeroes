
using System;
using UnityEngine;
using Muc.Data;
using System.Reflection;

// Supports floats and ints simultaneously

[Serializable]
public class NumericAttributeSelector {

	[field: SerializeField] private NumericAttributeFieldName field;

	[field: SerializeField] private float fallbackValue;
	[field: SerializeField] private float fallbackOther;
	[field: SerializeField] private bool fallbackEnabled = true;

	private object sourceCached;
	private string fieldNameCached;
	private AttributeBase attributeCached;

	public AttributeBase GetAttribute(object source) {
		UpdateCache(source);
		return attributeCached;
	}

	public float GetValue(object source) {
		UpdateCache(source);
		return attributeCached switch {
			Attribute<float> att => att.value,
			Attribute<int> att => att.value,
			_ => fallbackValue,
		};
	}
	public float GetRawValue(object source) {
		UpdateCache(source);
		return attributeCached switch {
			Attribute<float> att => att.rawValue,
			Attribute<int> att => att.rawValue,
			_ => fallbackValue,
		};
	}

	public float GetOther(object source) {
		UpdateCache(source);
		return attributeCached switch {
			DualAttribute<float> att => att.other,
			DualAttribute<int> att => att.other,
			_ => fallbackOther,
		};
	}
	public float GetRawOther(object source) {
		UpdateCache(source);
		return attributeCached switch {
			DualAttribute<float> att => att.rawOther,
			DualAttribute<int> att => att.rawOther,
			_ => fallbackOther,
		};
	}

	public bool GetEnabled(object source) {
		UpdateCache(source);
		return attributeCached switch {
			ToggleDualAttribute<float> att => att.enabled,
			ToggleDualAttribute<int> att => att.enabled,
			ToggleAttribute<float> att => att.enabled,
			ToggleAttribute<int> att => att.enabled,
			_ => fallbackEnabled,
		};
	}
	public bool GetRawEnabled(object source) {
		UpdateCache(source);
		return attributeCached switch {
			ToggleDualAttribute<float> att => att.rawEnabled,
			ToggleDualAttribute<int> att => att.rawEnabled,
			ToggleAttribute<float> att => att.rawEnabled,
			ToggleAttribute<int> att => att.rawEnabled,
			_ => fallbackEnabled,
		};
	}

	protected void UpdateCache(object source) {
#if UNITY_EDITOR
		if (!Application.isPlaying || (sourceCached != source || fieldNameCached != field.fieldName)) {
#else
		if (sourceCached != source || fieldNameCached != field.fieldName) {
#endif
			sourceCached = source;
			fieldNameCached = field.fieldName;
			attributeCached = null;
			if (!String.IsNullOrEmpty(field.fieldName)) {
				var fieldInfo = source.GetType().GetField(field.fieldName);
				if (fieldInfo != null) {
					var value = fieldInfo.GetValue(source);
					this.attributeCached = value switch {
						Attribute<float> att => att,
						Attribute<int> att => att,
						_ => null,
					};
				}
			}
		}
	}

#if DEBUG // Wow so defensive
	[Obsolete("Pass the containing object instead.")]
	public float GetValue(AttributeBase attribute) => throw new ArgumentException();
	[Obsolete("Pass the containing object instead.")]
	public float GetRawValue(AttributeBase attribute) => throw new ArgumentException();
	[Obsolete("Pass the containing object instead.")]
	public float GetOther(AttributeBase attribute) => throw new ArgumentException();
	[Obsolete("Pass the containing object instead.")]
	public float GetRawOther(AttributeBase attribute) => throw new ArgumentException();
	[Obsolete("Pass the containing object instead.")]
	public bool GetEnabled(AttributeBase attribute) => throw new ArgumentException();
	[Obsolete("Pass the containing object instead.")]
	public bool GetRawEnabled(AttributeBase attribute) => throw new ArgumentException();
#endif
}

#if UNITY_EDITOR
namespace Editors {
	using UnityEngine;
	using UnityEditor;
	using static Muc.Editor.PropertyUtil;
	using static Muc.Editor.EditorUtil;

	[CanEditMultipleObjects]
	[CustomPropertyDrawer(typeof(NumericAttributeSelector), true)]
	public class NumericAttributeSelectorDrawer : PropertyDrawer {

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			return property.isExpanded ? 5 * (lineHeight + spacing) : base.GetPropertyHeight(property, label);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			using (PropertyScope(position, label, property, out label)) {
				if (property.isExpanded) {
					EditorGUI.PropertyField(position, property, label, true);
				} else {
					var propPos = position;
					propPos.xMax = propPos.xMin + EditorGUIUtility.labelWidth + spacing;
					property.isExpanded = EditorGUI.Foldout(propPos, property.isExpanded, label, true);

					var fieldPos = position;
					fieldPos.xMin += EditorGUIUtility.labelWidth + spacing;
					var field = property.FindPropertyRelative("field");
					EditorGUI.PropertyField(fieldPos, field, GUIContent.none);
				}
			}
		}

	}
}
#endif