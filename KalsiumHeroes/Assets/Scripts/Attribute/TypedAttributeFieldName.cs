
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Reflection;
using Muc.Data;

[Serializable]
public class TypedAttributeFieldName {

	private static IEnumerable<(string, Type)> cache;

	[field: SerializeField] public string attributeName { get; private set; }
	[field: SerializeField] public SerializedType attributeType { get; private set; }

	public virtual IEnumerable<(string, Type)> GetFieldNames() {
		if (cache == null) {
			var list = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(v => v.GetTypes())
					.Where(v
						=> (v.IsClass || (v.IsValueType && !v.IsPrimitive))
						&& (v.IsSubclassOf(typeof(Object)) || v.CustomAttributes.Any(v => v.AttributeType == typeof(SerializableAttribute)))
					).SelectMany(v => v.GetFields()
						.Where(f => typeof(Attribute).IsAssignableFrom(f.FieldType))
						.Select(v => (v.Name, GetAttributeType(v.FieldType)))).Distinct().ToList();
			list.Sort((a, b) => a.Name.CompareTo(b.Name));
			cache = list;

		}
		return cache;
		Type GetAttributeType(Type fieldType) {
			while (fieldType != null) {
				if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(Attribute<>)) {
					return fieldType.GenericTypeArguments[0];
				}
				fieldType = fieldType.BaseType;
			}
			throw new ArgumentException("fieldType must extend Attribute<>", "fieldType");
		}
	}

}


#if UNITY_EDITOR
namespace Editors {

	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using Object = UnityEngine.Object;
	using static Muc.Editor.PropertyUtil;
	using static Muc.Editor.EditorUtil;

	[CanEditMultipleObjects]
	[CustomPropertyDrawer(typeof(TypedAttributeFieldName), true)]
	public class TypedAttributeFieldNameDrawer : PropertyDrawer {

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			var noLabel = label.text is "" && label.image is null;

			using (PropertyScope(position, label, property, out label)) {

				const int dropDownWidth = 20;

				var propertyPos = position;
				propertyPos.width -= dropDownWidth;

				var attributeName = property.FindPropertyRelative(GetBackingFieldName(nameof(TypedAttributeFieldName.attributeName)));
				EditorGUI.PropertyField(propertyPos, attributeName, label);

				var dropDownPos = position;
				dropDownPos.xMin += position.width - dropDownWidth;

				if (EditorGUI.DropdownButton(dropDownPos, GUIContent.none, FocusType.Keyboard)) {
					var attributeType = property.FindPropertyRelative(GetBackingFieldName(nameof(TypedAttributeFieldName.attributeType)));
					var type = GetFirstValue<SerializedType>(attributeType);
					var fnVal = GetFirstValue<TypedAttributeFieldName>(property);
					var pairs = fnVal.GetFieldNames();
					var menu = new GenericMenu();
					foreach (var pair in pairs) {
						menu.AddItem(
							new GUIContent($"{pair.Item1} ({pair.Item2.Name})"),
							attributeName.stringValue == pair.Item1,
							() => {
								attributeName.stringValue = pair.Item1;
								var types = GetValues<SerializedType>(attributeType);
								SetValue(attributeType, new SerializedType() { type = pair.Item2 });
								attributeName.serializedObject.ApplyModifiedProperties();
							}
						);
					}
					menu.DropDown(dropDownPos);
				}

			}
		}

	}
}
#endif