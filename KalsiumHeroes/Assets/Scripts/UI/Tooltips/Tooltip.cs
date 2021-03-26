
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Collections;
using UnityEngine.EventSystems;
using Muc.Extensions;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour {

	[SerializeField, Tooltip("The TooltipRoot used as the animation origin.")]
	public TooltipRoot rootPrefab;

	[HideInInspector] public int index;
	[HideInInspector] public string id;
	[HideInInspector] public TooltipRoot root;
	[HideInInspector] public GameObject creator;

	protected void OnDestroy() {
		Tooltips.instance.RemoveListing(this);
	}

	/// <summary> This is called when the user clicks on the source of this Tooltip. </summary>
	public virtual void OnCreatorClicked(Rect creatorRect) {

	}

	/// <summary> This is called when the tooltip is shown by Tooltips. </summary>
	public virtual void OnShow() {
		root.Show();
	}

	/// <summary> This is called when the tooltip is hidden by Tooltips. </summary>
	public virtual void OnHide() {
		if (root) root.Hide();
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
	[CustomEditor(typeof(Tooltip), true)]
	public class TooltipEditor : Editor {

		Tooltip t => (Tooltip)target;

		protected static List<MonoScript> tooltipScripts = null;
		protected static string[] options = null;

		protected int selected = -1;

		SerializedProperty scriptProp;

		void OnEnable() {
			scriptProp = serializedObject.FindProperty(script);

			if (tooltipScripts == null) {
				tooltipScripts = new List<MonoScript>();
				var options = new List<string>();
				var a = AssetDatabase.FindAssets("t:MonoScript");
				foreach (var item in a) {
					var path = AssetDatabase.GUIDToAssetPath(item);
					var asset = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
					var scriptType = asset.GetClass();
					if (scriptType == null) continue;
					if (scriptType == typeof(Tooltip) || scriptType.IsSubclassOf(typeof(Tooltip))) {
						tooltipScripts.Add(asset);
						options.Add(asset.name);
					}
				}
				TooltipEditor.options = options.ToArray();
			}

			int i = 0;
			foreach (var tooltipScript in tooltipScripts) {
				var scriptType = tooltipScript.GetClass();
				if (scriptType == t.GetType()) {
					selected = i;
				}
				i++;
			}

		}


		public override void OnInspectorGUI() {
			serializedObject.Update();

			if (selected != (selected = EditorGUILayout.Popup("Tooltip Type", selected, options)) && selected >= 0 && selected < options.Length) {
				scriptProp.objectReferenceValue = tooltipScripts[selected];
				serializedObject.ApplyModifiedProperties();
				return;
			}

			DrawDefaultInspector();

			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif