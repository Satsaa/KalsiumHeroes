
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Muc.Editor;
using Serialization;
using UnityEngine;

[RefToken]
public abstract class DataObject : ScriptableObject, IIdentifiable {

	[Tooltip("String identifier of this DataObject. (\"Unit_Oracle\")")]
	public string identifier;
	string IIdentifiable.identifier => identifier;

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
				if (Tooltips.instance.TooltipExists(converted) || current == typeof(DataObject)) {
					return _tooltip = converted;
				}
				current = current.BaseType;
			}
		}
	}

	[field: Tooltip("Source instance."), SerializeField]
	public DataObject source { get; protected set; }
	public bool isSource => source == null;

	[field: SerializeField]
	public bool removed { get; protected set; }

	[field: SerializeField, DoNotTokenize]
	public bool shown { get; set; }

	protected virtual void OnCreate() { }
	protected virtual void OnRemove() { }

	public void Show() {
		if (removed || shown == (shown = true)) return;
		OnShow();
	}
	public void Hide() {
		if (shown == (shown = false)) return;
		OnHide();
	}

	/// <summary> Create all GameObject related stuff here </summary>
	protected virtual void OnShow() { }
	/// <summary> Remove all GameObject related stuff here </summary>
	protected virtual void OnHide() { }

	/// <summary>
	/// Modifier is created or the scripts are reloaded.
	/// Also when the Modifier is removed but with add = false.
	/// Conditionally add or remove non-persistent things here.
	/// </summary>
	/// <param name="add"></param>
	protected virtual void OnConfigureNonpersistent(bool add) { }

#if UNITY_EDITOR
	[UnityEditor.Callbacks.DidReloadScripts]
	private static void OnReloadScripts() {
		if (Application.isPlaying && Game.game) {
			foreach (var dobj in Game.dataObjects.Get<DataObject>().Where(v => v)) {
				dobj.OnConfigureNonpersistent(true);
			}
		}
	}
#endif

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
	using Muc.Data;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(DataObject), true)]
	public class DataObjectEditor : Editor {

		DataObject t => (DataObject)target;

		static string[] excludes = { script };
		static List<bool> expands = new() { true };
		ReorderableList list;
		Type listType;

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
					EditorGUILayout.PropertyField(property, true);
				}
			}
			if (expandI != 0) {
				EditorGUI.indentLevel--;
			}

			serializedObject.ApplyModifiedProperties();
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
			createTypes ??= AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(v => v.GetTypes())
				.Where(v =>
					v.IsClass
					&& !v.IsAbstract
					&& (v == typeof(DataObject) || typeof(DataObject).IsAssignableFrom(v))
				).ToList();
			return createTypes.Where(v => v == createType || createType.IsAssignableFrom(v));
		}

	}
}
#endif