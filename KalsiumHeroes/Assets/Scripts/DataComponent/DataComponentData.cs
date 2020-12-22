
using UnityEngine;
using Muc.Data;

public abstract class DataComponentData : ScriptableObject {

	[Header("Data Component Data")]
	[Tooltip("Display name of this DataComponent, displayed to users. (\"Oracle\")")]
	public string displayName;

	[Tooltip("Description of this DataComponent, displayed to users.")]
	public string description;

	[Tooltip("String identifier of this DataComponent. (\"unit_oracle\")")]
	public string identifier;

	[Tooltip("Instantiate this Type of DataComponent for this data.")]
	public SerializedType<DataComponent> componentType;

}

#if UNITY_EDITOR
namespace Muc.Data {

	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using static Muc.Editor.PropertyUtil;
	using static Muc.Editor.EditorUtil;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(DataComponentData), true)]
	public class DataComponentDataDrawer : Editor {

		private SerializedProperty displayName;
		private SerializedProperty description;
		private SerializedProperty identifier;
		private SerializedProperty componentType;

		void OnEnable() {
			displayName = serializedObject.FindProperty(nameof(displayName));
			description = serializedObject.FindProperty(nameof(description));
			identifier = serializedObject.FindProperty(nameof(identifier));
			componentType = serializedObject.FindProperty(nameof(componentType));
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();

			EditorGUILayout.PropertyField(displayName);
			EditorGUILayout.PropertyField(description);
			EditorGUILayout.PropertyField(identifier);

			// Draw custom type selector
			var position = EditorGUILayout.GetControlRect();
			using (PropertyScope(position, new GUIContent(componentType.displayName), componentType, out var label)) {

				var values = GetValues<SerializedType>(componentType);
				var value = values.First();

				// Label
				EditorGUI.LabelField(position, label);
				position.xMin += EditorGUIUtility.labelWidth + spacing;
				// Dropdown
				var hint = new GUIContent(label) { text = value.type == null ? "null" : $"{value.type} ({value.type.Assembly.GetName().Name})" }; // Inherit state from label
				if (EditorGUI.DropdownButton(position, new GUIContent(hint), FocusType.Keyboard)) {
					var types = GetCompatibleTypes(value.type);
					var menu = TypeSelectMenu(types.ToList(), values.Select(v => v.type), type => OnSelect(componentType, type));
					menu.DropDown(position);
				}
			}

			DrawPropertiesExcluding(
				serializedObject,
				nameof(displayName),
				nameof(description),
				nameof(identifier),
				nameof(componentType)
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


		private static List<Type> dataComponentTypes;

		private static IEnumerable<Type> GetCompatibleTypes(Type dataType) {
			DataComponentDataDrawer.dataComponentTypes ??= AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(v => v.GetTypes())
				.Where(v =>
					(v.IsClass && !v.IsAbstract) &&
					(v == typeof(DataComponent) || typeof(DataComponent).IsAssignableFrom(v))
				).ToList();
			return DataComponentDataDrawer.dataComponentTypes.Where(v => IsCompatible(dataType, v));
		}

		// https://stackoverflow.com/questions/11162652/c-sharp-get-property-value-without-creating-instance
		private static bool IsCompatible(Type dataType, Type componentType) {

			var method = componentType.GetProperty(nameof(DataComponent.dataType)).GetGetMethod();
			var openDelegate = (Func<DataComponent, Type>)Delegate.CreateDelegate(typeof(Func<DataComponent, Type>), method);
			var otherDataType = openDelegate(null);
			return otherDataType.IsAssignableFrom(dataType);
		}


	}
}
#endif
