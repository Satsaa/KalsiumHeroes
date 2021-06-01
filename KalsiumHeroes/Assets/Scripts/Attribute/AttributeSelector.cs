
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

	private string fieldNameCache;
	private FieldInfo fieldInfoCache;

	[Obsolete("Pass the containing object instead.")]
	public T GetValue(Attribute<T> attribute) => throw new ArgumentException();
	public T GetValue(object source) {
		if (!String.IsNullOrEmpty(field.fieldName)) {
			if (fieldNameCache != field.fieldName || fieldInfoCache == null) {
				fieldInfoCache = source.GetType().GetField(field.fieldName);
				fieldNameCache = field.fieldName;
			}
			var fieldInfo = fieldInfoCache;
			if (fieldInfo != null) {
				var value = fieldInfo.GetValue(source);
				if (value is Attribute<T> attribute) {
					return attribute.value;
				}
			}
		}
		return fallbackValue;
	}

	[Obsolete("Pass the containing object instead.")]
	public T GetOther(Attribute<T> attribute) => throw new ArgumentException();
	public T GetOther(object source) {
		if (!String.IsNullOrEmpty(field.fieldName)) {
			if (fieldNameCache != field.fieldName || fieldInfoCache == null) {
				fieldInfoCache = source.GetType().GetField(field.fieldName);
				fieldNameCache = field.fieldName;
			}
			var fieldInfo = fieldInfoCache;
			if (fieldInfo != null) {
				var value = fieldInfo.GetValue(source);
				if (value is DualAttribute<T> attribute) {
					return attribute.other;
				}
			}
		}
		return fallbackOther;
	}

	[Obsolete("Pass the containing object instead.")]
	public bool GetEnabled(Attribute<T> attribute) => throw new ArgumentException();
	public bool GetEnabled(object source) {
		if (!String.IsNullOrEmpty(field.fieldName)) {
			if (fieldNameCache != field.fieldName || fieldInfoCache == null) {
				fieldInfoCache = source.GetType().GetField(field.fieldName);
				fieldNameCache = field.fieldName;
			}
			var fieldInfo = fieldInfoCache;
			if (fieldInfo != null) {
				var value = fieldInfo.GetValue(source);
				if (value is Attribute<T> attribute) {
					if (attribute is ToggleDualAttribute<T> tglDual) {
						return tglDual.enabled;
					}
					if (attribute is ToggleAttribute<T> tgl) {
						return tgl.enabled;
					}
				}
			}
		}
		return fallbackEnabled;
	}

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

					// Label
					property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);
					position.xMin += EditorGUIUtility.labelWidth + spacing;

					var field = property.FindPropertyRelative("field");
					EditorGUI.PropertyField(position, field, GUIContent.none);
				}
			}
		}

	}
}
#endif