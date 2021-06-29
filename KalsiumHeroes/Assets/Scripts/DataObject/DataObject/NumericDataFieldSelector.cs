﻿
using System;
using UnityEngine;
using Muc.Data;
using System.Reflection;

// Supports floats and ints simultaneously

[Serializable]
public class NumericDataFieldSelector {

	[field: SerializeField] private NumericDataFieldName field;

	[field: SerializeField] private float fallbackValue;
	[field: SerializeField] private float fallbackOther;
	[field: SerializeField] private bool fallbackEnabled = true;

	[field: SerializeField, Tooltip("Override value if the attribute is disabled.")] private ToggleValue<float> overrideValue;
	[field: SerializeField, Tooltip("Override other if the attribute is disabled.")] private ToggleValue<float> overrideOther;

	[field: SerializeField, Tooltip("If enabled, getting the primary value returns the secondary value, and vice versa.")]
	private bool swap;

	private object sourceCached;
	private string fieldNameCached;
	private AttributeBase attributeCached;
	private FieldInfo fieldCached;

	public string GetFieldName() => field.attributeName;
	public Type GetFieldType() => field.attributeType.type;

	public object GetFieldValue(object source) {
		UpdateCache(source);
		if (fieldCached != null) return fieldCached.GetValue(source);
		return attributeCached;
	}

	public float GetValue(object source, bool ignoreSwap = false) {
		if (swap && !ignoreSwap) return GetOther(source, true);
		UpdateCache(source);
		if (fieldCached != null) return AsFloat(fieldCached.GetValue(source));
		return TryOverrideValue(source, attributeCached switch {
			Attribute<float> att => att.value,
			Attribute<int> att => att.value,
			_ => fallbackValue,
		});
	}
	public float GetRawValue(object source, bool ignoreSwap = false) {
		if (swap && !ignoreSwap) return GetRawOther(source, true);
		UpdateCache(source);
		if (fieldCached != null) return AsFloat(fieldCached.GetValue(source));
		return TryOverrideValue(source, attributeCached switch {
			Attribute<float> att => att.rawValue,
			Attribute<int> att => att.rawValue,
			_ => fallbackValue,
		});
	}

	public float GetOther(object source, bool ignoreSwap = false) {
		if (swap && !ignoreSwap) return GetValue(source, true);
		UpdateCache(source);
		return TryOverrideOther(source, attributeCached switch {
			DualAttribute<float> att => att.other,
			DualAttribute<int> att => att.other,
			_ => fallbackOther,
		});
	}
	public float GetRawOther(object source, bool ignoreSwap = false) {
		if (swap && !ignoreSwap) return GetRawValue(source, true);
		UpdateCache(source);
		return TryOverrideOther(source, attributeCached switch {
			DualAttribute<float> att => att.rawOther,
			DualAttribute<int> att => att.rawOther,
			_ => fallbackOther,
		});
	}

	public bool GetEnabled(object source) {
		UpdateCache(source);
		if (fieldCached != null) {
			var value = fieldCached.GetValue(source);
			if (value is ToggleValue<int> tv1) return tv1.enabled;
			if (value is ToggleValue<float> tv2) return tv2.enabled;
		}
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
		if (fieldCached != null) {
			var value = fieldCached.GetValue(source);
			if (value is ToggleValue<int> tv1) return tv1.enabled;
			if (value is ToggleValue<float> tv2) return tv2.enabled;
		}
		return attributeCached switch {
			ToggleDualAttribute<float> att => att.rawEnabled,
			ToggleDualAttribute<int> att => att.rawEnabled,
			ToggleAttribute<float> att => att.rawEnabled,
			ToggleAttribute<int> att => att.rawEnabled,
			_ => fallbackEnabled,
		};
	}

	protected float AsFloat(object value) {
		return value switch {
			float v => v,
			int v => (float)v,
			_ => default,
		};
	}

	protected float TryOverrideValue(object source, float value) {
		return !overrideValue.enabled || GetEnabled(source) ? value : overrideValue.value;
	}

	protected float TryOverrideOther(object source, float value) {
		return !overrideOther.enabled || GetEnabled(source) ? value : overrideOther.value;
	}

	protected void UpdateCache(object source) {
#if UNITY_EDITOR
		if (!Application.isPlaying || (sourceCached != source || fieldNameCached != field.attributeName)) {
#else
		if (sourceCached != source || fieldNameCached != field.fieldName) {
#endif
			sourceCached = source;
			fieldNameCached = field.attributeName;
			attributeCached = null;
			fieldCached = null;
			if (!String.IsNullOrEmpty(field.attributeName)) {
				var fieldInfo = source.GetType().GetField(field.attributeName);
				if (fieldInfo != null) {
					var value = fieldInfo.GetValue(source);
					if (value is AttributeBase) {
						this.attributeCached = value switch {
							Attribute<float> att => att,
							Attribute<int> att => att,
							_ => null,
						};
					} else {
						fieldCached = fieldInfo;
					}

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
	[CustomPropertyDrawer(typeof(NumericDataFieldSelector), true)]
	public class NumericDataFieldSelectorDrawer : PropertyDrawer {

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			return property.isExpanded ? 8 * (lineHeight + spacing) : base.GetPropertyHeight(property, label);
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