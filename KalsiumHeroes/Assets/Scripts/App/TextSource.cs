using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(TextSource), menuName = nameof(TextSource))]
public class TextSource : ScriptableObject {

	[Serializable]
	protected class Pair {
		public TextAsset asset;
		public Lang lang;
	}

	[SerializeField] protected List<Pair> assets;

	protected void Reset() {
		assets = new List<Pair>();
		foreach (Lang lang in Enum.GetValues(typeof(Lang))) {
			assets.Add(new Pair() { lang = lang });
		}
	}

	public string GetText() {
		var asset = assets.Find(v => v.lang == App.lang).asset;
		if (asset == null) return "";
		return asset.text;
	}

}

namespace Editors {

	using UnityEngine;
	using UnityEditor;
	using UnityEditorInternal;

	using static Muc.Editor.EditorUtil;
	using static Muc.Editor.PropertyUtil;
	using System.Linq;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TextSource))]
	public class TextSourceEditor : Editor {

		SerializedProperty assets;

		ReorderableList list;
		HashSet<Lang> seen = new HashSet<Lang>();

		private void OnEnable() {
			assets = serializedObject.FindProperty(nameof(assets));

			list = new ReorderableList(serializedObject, assets, true, true, true, true);

			list.drawElementCallback = DrawListItems;

		}

		public override void OnInspectorGUI() {
			serializedObject.Update();
			DrawPropertiesExcluding(serializedObject, nameof(assets));
			seen.Clear();
			list.DoLayoutList();
			serializedObject.ApplyModifiedProperties();
		}

		void DrawListItems(Rect rect, int index, bool isActive, bool isFocused) {
			SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);

			var pos = rect;
			pos.height = lineHeight;

			pos.yMin += spacing / 2f;
			pos.yMax += spacing / 2f;

			pos.width = 55f;

			var lang = element.FindPropertyRelative("lang");
			var val = GetFirstValue<Lang>(lang);
			if (seen.Contains(val)) {
				using (ColorScope(v => serializedObject.isEditingMultipleObjects ? Color.Lerp(v, Color.red, 0.5f) : Color.red)) {
					EditorGUI.PropertyField(pos, lang, GUIContent.none);
				}
			} else {
				seen.Add(val);
				EditorGUI.PropertyField(pos, element.FindPropertyRelative("lang"), GUIContent.none);
			}

			pos.xMin = pos.xMax + spacing;
			pos.xMax = rect.xMax;

			EditorGUI.PropertyField(pos, element.FindPropertyRelative("asset"), GUIContent.none);

		}

	}
}