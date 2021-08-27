
using UnityEngine;
using Muc.Data;
using System;
using UnityEngine.Serialization;
using System.Text.RegularExpressions;
using Serialization;

[RefToken]
[CreateAssetMenu(fileName = nameof(DataObjectData), menuName = "DataSources/" + nameof(DataObjectData))]
public abstract class DataObjectData : ScriptableObject, IIdentifiable {

	/// <summary> Base type for createType. </summary>
	public abstract Type createTypeConstraint { get; }

	[Tooltip("Create this Type of DataObject for this data.")]
	public SerializedType<DataObject> createType;

	[Tooltip("String identifier of this DataObject. (\"Unit_Oracle\")")]
	public string identifier;
	string IIdentifiable.identifier => identifier;

	bool _isSource;
	public bool isSource {
		get {
			App.library.UpdateSources();
			return _isSource;
		}
		internal set => _isSource = value;
	}

	private static Regex removeData = new(@"Data$");
	private string _tooltip;
	public string tooltip {
		get {
			if (_tooltip != null) return _tooltip;
			var identifierTooltip = $"{identifier}_Info";
			if (Tooltips.instance.TooltipExists(identifierTooltip)) {
				return _tooltip = identifierTooltip;
			}
			var current = this.GetType();
			while (true) {
				var converted = current.FullName;
				converted = removeData.Replace(converted, "");
				converted += "_Info";
				if (Tooltips.instance.TooltipExists(converted) || current == typeof(DataObjectData)) {
					return _tooltip = converted;
				}
				current = current.BaseType;
			}
		}
	}

	protected virtual void OnValidate() {
#if UNITY_EDITOR
		isSource = UnityEditor.AssetDatabase.Contains(this);
#endif
		if (createType.type == null) {
			if (!createTypeConstraint.IsAbstract) {
				createType.type = createTypeConstraint;
			}
		}
	}

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

		private SerializedProperty createType;
		private SerializedProperty baseModifiers; // Only on MasterData

		private DataObjectData t => (DataObjectData)target;

		static String[] excludes = { "m_Script" };
		static List<bool> expands = new List<bool>() { true };
		ReorderableList list;
		Type listType;

		void OnEnable() {
			createType = serializedObject.FindProperty(nameof(createType));
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
						EditorGUI.indentLevel--;
					}
					expands[expandI] = EditorGUILayout.Foldout(expands[expandI], ObjectNames.NicifyVariableName(decType.Name), true, EditorStyles.foldoutHeader);
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
						case nameof(createType):
							DrawDataObjectType();
							break;
						default:
							EditorGUILayout.PropertyField(property, true);
							break;
					}
				}
			}
			if (expandI != 0) {
				EditorGUI.indentLevel--;
			}

			serializedObject.ApplyModifiedProperties();
		}

		private void DrawDataObjectType() {
			// Draw custom type selector
			var position = EditorGUILayout.GetControlRect();
			using (PropertyScope(position, new GUIContent(createType.displayName), createType, out var label)) {

				var values = GetValues<SerializedType>(createType);
				var value = values.First();

				// Label
				EditorGUI.LabelField(position, label);
				position.xMin += EditorGUIUtility.labelWidth + spacing;
				// Dropdown
				var hint = new GUIContent(label) { text = value.type == null ? "Unselected!" : $"{value.type} ({value.type.Assembly.GetName().Name})" }; // Inherit state from label
				if (EditorGUI.DropdownButton(position, new GUIContent(hint), FocusType.Keyboard)) {
					var types = GetCompatibleTypes(value.type, t.createTypeConstraint);
					var menu = TypeSelectMenu(types.ToList(), values.Select(v => v.type), type => OnSelect(createType, type));
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


		private static List<Type> createTypes;

		private static IEnumerable<Type> GetCompatibleTypes(Type dataType, Type createType) {
			DataObjectDataDrawer.createTypes ??= AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(v => v.GetTypes())
				.Where(v =>
					(v.IsClass && !v.IsAbstract) &&
					(v == typeof(DataObject) || typeof(DataObject).IsAssignableFrom(v))
				).ToList();
			return createTypes.Where(v => v == createType || createType.IsAssignableFrom(v));
		}


		protected internal static void DrawPropsExcluding(SerializedObject obj, params string[] propertyToExclude) {
		}
	}
}
#endif
