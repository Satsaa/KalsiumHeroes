
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Collections;
using UnityEngine.EventSystems;
using Muc.Extensions;
using UnityEngine.UI;
using Muc.Components.Extended;

[RequireComponent(typeof(TooltipAnimator))]
public class Tooltip : ExtendedUIBehaviour {

	[SerializeField] bool windowizable = true;
	[Tooltip("The Window prefab used for windowization. Leave empty for default defined by Tooltips.")]
	[SerializeField] Window windowPrefab;
	[Tooltip("List of objects to destroy if windowized.")]
	[SerializeField] List<Object> destroyOnWindowize;

	[HideInInspector] public int index;
	[HideInInspector] public string query;
	[HideInInspector] public RectTransform creator;
	[HideInInspector] public bool windowized;

	[SerializeField, HideInInspector] TooltipAnimator tooltipAnimator;

	new protected void Awake() {
		base.Awake();
		tooltipAnimator = GetComponent<TooltipAnimator>();
	}

	new protected void OnDestroy() {
		base.OnDestroy();
		Tooltips.instance.RemoveListing(this);
	}

	/// <summary> This is called when the user clicks on the source of this Tooltip. </summary>
	public virtual void OnCreatorClicked(Rect creatorRect) {
		Windowize(windowPrefab);
	}

	/// <summary> This is called when the tooltip is shown by Tooltips. </summary>
	public virtual void OnShow() {
		tooltipAnimator.Show();
	}

	/// <summary> This is called when the tooltip is hidden by Tooltips. </summary>
	public virtual void OnHide() {
		if (windowized) return;
		tooltipAnimator.Hide();
	}

	protected virtual void Windowize(Window windowPrefab) {
		if (!windowizable) return;
		if (gameObject.GetComponentInParent<Window>()) return;
		rectTransform.ShiftPivot(new Vector2(0.5f, 0.5f));
		windowized = true;
		tooltipAnimator.FinishAnims();
		var window = Instantiate(windowPrefab ? windowPrefab : Tooltips.instance.defaultWindowPrefab, gameObject.transform.position, gameObject.transform.rotation, Windows.rectTransform);
		Windows.MoveToTop(window.gameObject.transform);
		transform.SetParent(window.content.contentParent);
		foreach (var destroy in destroyOnWindowize) {
			Destroy(destroy);
		}
		window.gameObject.transform.Translate(0, window.toolbar.rectTransform.sizeDelta.y * window.toolbar.rectTransform.lossyScale.y / 2, 0);
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

		SerializedProperty windowizable;
		SerializedProperty windowPrefab;
		SerializedProperty destroyOnWindowize;

		void OnEnable() {

			scriptProp = serializedObject.FindProperty(script);

			windowizable = serializedObject.FindProperty(nameof(windowizable));
			windowPrefab = serializedObject.FindProperty(nameof(windowPrefab));
			destroyOnWindowize = serializedObject.FindProperty(nameof(destroyOnWindowize));

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

			EditorGUILayout.PropertyField(windowizable);
			if (windowizable.boolValue) {
				EditorGUILayout.PropertyField(windowPrefab);
				EditorGUILayout.PropertyField(destroyOnWindowize);
			}

			DrawPropertiesExcluding(serializedObject,
				script,
				windowizable.name,
				windowPrefab.name,
				destroyOnWindowize.name
			);

			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif