
using UnityEngine;
using Muc.Data;
using System;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = nameof(DataObjectData), menuName = "DataSources/" + nameof(DataObjectData))]
public abstract class DataObjectData : ScriptableObject {

	/// <summary> Base type for createType. </summary>
	public abstract Type createTypeConstraint { get; }

	[Tooltip("Create this Type of DataObject for this data."), FormerlySerializedAs("componentType")]
	public SerializedType<DataObject> createType;

	[Tooltip("String identifier of this DataObject. (\"unit_oracle\")")]
	public string identifier;

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

		private SerializedProperty dataObjectType;
		private SerializedProperty baseModifiers; // Only on MasterData

		private DataObjectData t => (DataObjectData)target;

		static String[] excludes = { "m_Script" };
		static List<bool> expands = new List<bool>() { true };
		ReorderableList list;
		Type listType;

		void OnEnable() {
			dataObjectType = serializedObject.FindProperty(nameof(dataObjectType));
			baseModifiers = serializedObject.FindProperty(nameof(baseModifiers));

			if (baseModifiers != null) {
				list = new ReorderableList(baseModifiers.serializedObject, baseModifiers, true, false, true, true);
				var prop = t.createTypeConstraint.GetProperty(nameof(Unit.modifierDataType), BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Public);
				listType = (Type)prop.GetValue(null);
				list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
					var element = baseModifiers.GetArrayElementAtIndex(index);
					EditorGUI.ObjectField(rect, element, listType);
				};
			}
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();

			Type decType = null;
			int expandI = 0;

			var property = serializedObject.GetIterator();
			var expanded = true;
			while (property.NextVisible(expanded)) {
				expanded = false;
				if (excludes.Contains(property.name)) continue;
				var fi = GetFieldInfo(property);
				var prev = decType;
				if (decType != (decType = fi.DeclaringType)) {
					if (expandI != 0) {
						EditorGUILayout.EndFoldoutHeaderGroup();
						EditorGUI.indentLevel--;
					}
					expands[expandI] = EditorGUILayout.BeginFoldoutHeaderGroup(expands[expandI], ObjectNames.NicifyVariableName(decType.Name), EditorStyles.foldoutHeader);
					EditorGUI.indentLevel++;
					expandI++;
					if (expands.Count <= expandI) {
						expands.Add(true);
					}
				}
				if (expands[expandI - 1]) {
					switch (property.name) {
						case nameof(baseModifiers):
							DrawBaseModifiers();
							break;
						case nameof(dataObjectType):
							DrawDataObjectType();
							break;
						default:
							EditorGUILayout.PropertyField(property, true);
							break;
					}
				}
			}
			if (expandI != 0) {
				EditorGUILayout.EndFoldoutHeaderGroup();
				EditorGUI.indentLevel--;
			}

			serializedObject.ApplyModifiedProperties();
		}

		private void DrawDataObjectType() {
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
					var types = GetCompatibleTypes(value.type, t.createTypeConstraint);
					var menu = TypeSelectMenu(types.ToList(), values.Select(v => v.type), type => OnSelect(dataObjectType, type));
					menu.DropDown(position);
				}
			}
		}

		static bool expandBaseModifier = true;
		private void DrawBaseModifiers() {
			if (expandBaseModifier = EditorGUILayout.Foldout(expandBaseModifier, nameof(baseModifiers))) {
				list.DoLayoutList();
			}
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


		protected internal static void DrawPropsExcluding(SerializedObject obj, params string[] propertyToExclude) {
		}
	}
}
#endif
