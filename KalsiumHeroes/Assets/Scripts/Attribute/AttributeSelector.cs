﻿
using System;
using UnityEngine;
using Muc.Data;
using System.Reflection;

[Serializable]
public class AttributeSelector<T> {

	[field: SerializeField] private AttributeFieldName field;

	[field: SerializeField] private T fallbackValue;
	[field: SerializeField] private T fallbackOther;
	[field: SerializeField] private bool fallbackEnabled = true;

	private object sourceCached;
	private string fieldNameCached;
	private Attribute<T> attributeCached;

	public Attribute<T> GetAttribute(object source) {
		UpdateCache(source);
		return attributeCached;
	}

	public T GetValue(object source) {
		UpdateCache(source);
		if (attributeCached != null) return attributeCached.value;
		return fallbackValue;
	}
	public T GetRawValue(object source) {
		UpdateCache(source);
		if (attributeCached != null) return attributeCached.rawValue;
		return fallbackValue;
	}

	public T GetOther(object source) {
		UpdateCache(source);
		return attributeCached switch {
			DualAttribute<T> att => att.other,
			_ => fallbackOther,
		};
	}
	public T GetRawOther(object source) {
		UpdateCache(source);
		return attributeCached switch {
			DualAttribute<T> att => att.rawOther,
			_ => fallbackOther,
		};
	}

	public bool GetEnabled(object source) {
		UpdateCache(source);
		return attributeCached switch {
			ToggleDualAttribute<T> att => att.enabled,
			ToggleAttribute<T> att => att.enabled,
			_ => fallbackEnabled,
		};
	}
	public bool GetRawEnabled(object source) {
		UpdateCache(source);
		return attributeCached switch {
			ToggleDualAttribute<T> att => att.rawEnabled,
			ToggleAttribute<T> att => att.rawEnabled,
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
					if (value is Attribute<T> attribute) {
						attributeCached = attribute;
					}
				}
			}
		}
	}

#if DEBUG // Wow so defensive
	[Obsolete("Pass the containing object instead.")]
	public T GetValue(Attribute<T> attribute) => throw new ArgumentException();
	[Obsolete("Pass the containing object instead.")]
	public T GetRawValue(Attribute<T> attribute) => throw new ArgumentException();
	[Obsolete("Pass the containing object instead.")]
	public T GetOther(Attribute<T> attribute) => throw new ArgumentException();
	[Obsolete("Pass the containing object instead.")]
	public T GetRawOther(Attribute<T> attribute) => throw new ArgumentException();
	[Obsolete("Pass the containing object instead.")]
	public bool GetEnabled(Attribute<T> attribute) => throw new ArgumentException();
	[Obsolete("Pass the containing object instead.")]
	public bool GetRawEnabled(Attribute<T> attribute) => throw new ArgumentException();
#endif
}

#if UNITY_EDITOR
namespace Editors {
	using UnityEngine;
	using UnityEditor;
	using static Muc.Editor.PropertyUtil;
	using static Muc.Editor.EditorUtil;

	[CanEditMultipleObjects]
	[CustomPropertyDrawer(typeof(AttributeSelector<>), true)]
	public class AttributeSelectorDrawer : PropertyDrawer {

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