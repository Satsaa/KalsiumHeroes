
using System;
using UnityEngine;
using Muc.Data;
using System.Reflection;

// Supports floats and ints simultaneously

[Serializable]
public class TextDataFieldSelector {

	[field: SerializeField] private TextDataFieldName field;

	[field: SerializeField] private TextSource fallbackValue;
	[field: SerializeField] private TextSource fallbackOther;

	[field: SerializeField] private bool fallbackEnabled = true;

	[field: SerializeField, Tooltip("Override value if the attribute is disabled.")] private ToggleValue<TextSource> overrideValue;
	[field: SerializeField, Tooltip("Override other if the attribute is disabled.")] private ToggleValue<TextSource> overrideOther;

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

	public string GetValue(object source, bool ignoreSwap = false) {
		if (swap && !ignoreSwap) return GetOther(source, true);
		UpdateCache(source);
		if (fieldCached != null) return AsString(fieldCached.GetValue(source));
		return TryOverrideValue(source, attributeCached switch {
			Attribute<string> att => att.value,
			Attribute<TextSource> att => att.value,
			_ => fallbackValue,
		});
	}
	public string GetRawValue(object source, bool ignoreSwap = false) {
		if (swap && !ignoreSwap) return GetRawOther(source, true);
		UpdateCache(source);
		if (fieldCached != null) return AsString(fieldCached.GetValue(source));
		return TryOverrideValue(source, attributeCached switch {
			Attribute<string> att => att.rawValue,
			Attribute<TextSource> att => att.rawValue,
			_ => fallbackValue,
		});
	}

	public string GetOther(object source, bool ignoreSwap = false) {
		if (swap && !ignoreSwap) return GetValue(source, true);
		UpdateCache(source);
		return TryOverrideOther(source, attributeCached switch {
			DualAttribute<string> att => att.other,
			DualAttribute<TextSource> att => att.other,
			_ => fallbackOther,
		});
	}
	public string GetRawOther(object source, bool ignoreSwap = false) {
		if (swap && !ignoreSwap) return GetRawValue(source, true);
		UpdateCache(source);
		return TryOverrideOther(source, attributeCached switch {
			DualAttribute<string> att => att.rawOther,
			DualAttribute<TextSource> att => att.rawOther,
			_ => fallbackOther,
		});
	}

	public bool GetEnabled(object source) {
		UpdateCache(source);
		if (fieldCached != null) {
			var value = fieldCached.GetValue(source);
			if (value is ToggleValue<TextSource> tv1) return tv1.enabled;
			if (value is ToggleValue<string> tv2) return tv2.enabled;
		}
		return attributeCached switch {
			ToggleDualAttribute<string> att => att.enabled,
			ToggleDualAttribute<TextSource> att => att.enabled,
			ToggleAttribute<string> att => att.enabled,
			ToggleAttribute<TextSource> att => att.enabled,
			_ => fallbackEnabled,
		};
	}
	public bool GetRawEnabled(object source) {
		UpdateCache(source);
		if (fieldCached != null) {
			var value = fieldCached.GetValue(source);
			if (value is ToggleValue<TextSource> tv1) return tv1.enabled;
			if (value is ToggleValue<string> tv2) return tv2.enabled;
		}
		return attributeCached switch {
			ToggleDualAttribute<string> att => att.rawEnabled,
			ToggleDualAttribute<TextSource> att => att.rawEnabled,
			ToggleAttribute<string> att => att.rawEnabled,
			ToggleAttribute<TextSource> att => att.rawEnabled,
			_ => fallbackEnabled,
		};
	}

	protected string AsString(object value) {
		return value switch {
			string v => v,
			TextSource v => v,
			_ => default,
		};
	}

	protected string TryOverrideValue(object source, string value) {
		return !overrideValue.enabled || GetEnabled(source) ? value : overrideValue.value;
	}

	protected string TryOverrideOther(object source, string value) {
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
							Attribute<string> att => att,
							Attribute<TextSource> att => att,
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
	public string GetValue(AttributeBase attribute) => throw new ArgumentException();
	[Obsolete("Pass the containing object instead.")]
	public string GetRawValue(AttributeBase attribute) => throw new ArgumentException();
	[Obsolete("Pass the containing object instead.")]
	public string GetOther(AttributeBase attribute) => throw new ArgumentException();
	[Obsolete("Pass the containing object instead.")]
	public string GetRawOther(AttributeBase attribute) => throw new ArgumentException();
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
	[CustomPropertyDrawer(typeof(TextDataFieldSelector), true)]
	public class TextDataFieldSelectorDrawer : PropertyDrawer {

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