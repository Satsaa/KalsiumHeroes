
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

	[field: SerializeField, Tooltip("Override value if the attribute is disabled.")] private ToggleValue<T> overrideValue;
	[field: SerializeField, Tooltip("Override other if the attribute is disabled.")] private ToggleValue<T> overrideOther;

	private object sourceCached;
	private string fieldNameCached;
	private Attribute<T> ac;

	public Attribute<T> GetAttribute(object source) {
		UpdateCache(source);
		return ac;
	}

	public T GetValue(object source) {
		UpdateCache(source);
		if (ac != null) return TryOverrideValue(source, ac.value);
		return TryOverrideValue(source, fallbackValue);
	}
	public T GetRawValue(object source) {
		UpdateCache(source);
		if (ac != null) return TryOverrideValue(source, ac.rawValue);
		return TryOverrideValue(source, fallbackValue);
	}

	public T GetOther(object source) {
		UpdateCache(source);
		return TryOverrideOther(source, ac != null && ac.HasOther() ? ac.GetOther() : fallbackOther);
	}
	public T GetRawOther(object source) {
		UpdateCache(source);
		return TryOverrideOther(source, ac != null && ac.HasOther() ? ac.GetRawOther() : fallbackOther);
	}

	public bool GetEnabled(object source) {
		UpdateCache(source);
		return ac != null && ac.HasEnabled() ? ac.GetEnabled() : fallbackEnabled;
	}
	public bool GetRawEnabled(object source) {
		UpdateCache(source);
		return ac != null && ac.HasEnabled() ? ac.GetRawEnabled() : fallbackEnabled;
	}

	protected T TryOverrideValue(object source, T value) {
		return !overrideValue.enabled || GetEnabled(source) ? value : overrideValue.value;
	}

	protected T TryOverrideOther(object source, T value) {
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
			ac = null;
			if (!String.IsNullOrEmpty(field.attributeName)) {
				var fieldInfo = source.GetType().GetField(field.attributeName);
				if (fieldInfo != null) {
					var value = fieldInfo.GetValue(source);
					if (value is Attribute<T> attribute) {
						ac = attribute;
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
			return property.isExpanded ? 7 * (lineHeight + spacing) : base.GetPropertyHeight(property, label);
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