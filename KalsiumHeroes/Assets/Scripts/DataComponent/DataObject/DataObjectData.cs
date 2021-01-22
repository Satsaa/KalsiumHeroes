
using UnityEngine;
using Muc.Data;
using System;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = nameof(DataObjectData), menuName = "DataSources/" + nameof(DataObjectData))]
public abstract class DataObjectData : ScriptableObject {

	/// <summary> Base type for objectType. </summary>
	public abstract Type ownerConstraint { get; }

	[Header("Data Object Data")]

	[Tooltip("String identifier of this DataObject. (\"unit_oracle\")")]
	public string identifier;

	[Tooltip("Create this Type of DataObject for this data."), FormerlySerializedAs("componentType")]
	public SerializedType<DataObject> dataObjectType;

}

#if UNITY_EDITOR
namespace Editors {

	using System;
	using System.Linq;
	using System.Collections.Generic;
	using System.Reflection;
	using UnityEngine;
	using UnityEditor;
	using static Muc.Editor.PropertyUtil;
	using static Muc.Editor.EditorUtil;
	using UnityEditorInternal;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(DataObjectData), true)]
	public class DataObjectDataDrawer : Editor {

		private SerializedProperty identifier;
		private SerializedProperty dataObjectType;
		private SerializedProperty container;
		private SerializedProperty baseModifiers; // Only on MasterData

		private DataObjectData t => (DataObjectData)target;

		ReorderableList list;
		Type listType;

		void OnEnable() {
			identifier = serializedObject.FindProperty(nameof(identifier));
			dataObjectType = serializedObject.FindProperty(nameof(dataObjectType));
			container = serializedObject.FindProperty(nameof(container));
			baseModifiers = serializedObject.FindProperty(nameof(baseModifiers));

			if (baseModifiers != null) {
				list = new ReorderableList(baseModifiers.serializedObject, baseModifiers);
				var prop = t.ownerConstraint.GetProperty(nameof(Unit.modifierDataType), BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Public);
				listType = (Type)prop.GetValue(null);
				list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
					var element = baseModifiers.GetArrayElementAtIndex(index);
					EditorGUI.ObjectField(rect, element, listType);
				};
			}
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();

			EditorGUILayout.PropertyField(identifier);

			// Draw custom type selector
			var position = EditorGUILayout.GetControlRect();
			using (PropertyScope(position, new GUIContent(dataObjectType.displayName), dataObjectType, out var label)) {

				var values = GetValues<SerializedType>(dataObjectType);
				var value = values.First();

				// Label
				EditorGUI.LabelField(position, label);
				position.xMin += EditorGUIUtility.labelWidth + spacing;
				// Dropdown
				var hint = new GUIContent(label) { text = value.type == null ? "null" : $"{value.type} ({value.type.Assembly.GetName().Name})" }; // Inherit state from label
				if (EditorGUI.DropdownButton(position, new GUIContent(hint), FocusType.Keyboard)) {
					var types = GetCompatibleTypes(value.type, t.ownerConstraint);
					var menu = TypeSelectMenu(types.ToList(), values.Select(v => v.type), type => OnSelect(dataObjectType, type));
					menu.DropDown(position);
				}
			}

			if (container != null) {
				EditorGUILayout.PropertyField(container);
			}
			if (baseModifiers != null) {
				list.DoLayoutList();
			}

			DrawPropertiesExcluding(
				serializedObject,
				"m_Script",
				nameof(identifier),
				nameof(dataObjectType),
				nameof(container),
				nameof(baseModifiers)
			);

			serializedObject.ApplyModifiedProperties();
		}


		private static void OnSelect(SerializedProperty property, Type type) {
			var values = GetValues<SerializedType>(property);
			Undo.RecordObjects(property.serializedObject.targetObjects, $"Set {property.name}");
			foreach (var value in values) value.type = type;
			foreach (var target in property.serializedObject.targetObjects) EditorUtility.SetDirty(target);
			property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
		}


		private static List<Type> dataObjectTypes;

		private static IEnumerable<Type> GetCompatibleTypes(Type dataType, Type dataObjectType) {
			DataObjectDataDrawer.dataObjectTypes ??= AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(v => v.GetTypes())
				.Where(v =>
					(v.IsClass && !v.IsAbstract) &&
					(v == typeof(DataObject) || typeof(DataObject).IsAssignableFrom(v))
				).ToList();
			return dataObjectTypes.Where(v => v == dataObjectType || dataObjectType.IsAssignableFrom(v));
		}


	}
}
#endif
