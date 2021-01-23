using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(TextSource), menuName = nameof(TextSource))]
public class TextSource : ScriptableObject {

	public static implicit operator string(TextSource v) => v ? v.text : null;

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

	protected string text {
		get {
			var asset = assets.Find(v => v.lang == App.lang).asset;
			if (asset == null) return "";
			return asset.text;
		}

	}

	public void SetText(Lang lang, TextAsset asset) {
		var first = assets.First(v => v.lang == lang);
		if (first == null) {
			assets.Add(new Pair() { asset = asset, lang = lang });
		} else {
			first.asset = asset;
		}
	}

}


#if UNITY_EDITOR
namespace Editors {

	using System;
	using System.IO;
	using System.Linq;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;
	using UnityEngine;
	using UnityEditor;
	using static Muc.Editor.PropertyUtil;
	using static Muc.Editor.EditorUtil;

	[CanEditMultipleObjects]
	[CustomPropertyDrawer(typeof(TextSource))]
	public class TextSourceDrawer : PropertyDrawer {

		const float buttonHeight = 22;
		static Regex identifierRegex = new Regex(@"(^[^_]*)_(.*)");
		static Regex whitespaceRegex = new Regex(@"\s+");

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			if (property.isExpanded) {
				return EditorGUI.GetPropertyHeight(property, label, true) + buttonHeight;
			} else {
				return EditorGUI.GetPropertyHeight(property, label, true);
			}
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			using (PropertyScope(position, label, property, out label)) {

				var lineRect = position;
				lineRect.height = lineHeight;

				EditorGUI.PropertyField(lineRect, property, label);
				if (property.isExpanded = EditorGUI.Foldout(lineRect, property.isExpanded, GUIContent.none, true)) {
					EditorGUI.PropertyField(lineRect, property, label, true);
					using (IndentScope()) {
						var buttonRect = position;
						buttonRect.yMin = buttonRect.yMax - buttonHeight + spacing;
						buttonRect = EditorGUI.IndentedRect(buttonRect);
						if (GUI.Button(buttonRect, "Generate Text Assets")) {
							var identifier = property.serializedObject.FindProperty("identifier");
							if (identifier == null || String.IsNullOrWhiteSpace(identifier.stringValue)) {
								Debug.LogError("No defined identifier on Object.");
							} else {
								var matches = identifierRegex.Match(identifier.stringValue);
								if (matches.Groups.Count != 3) {
									Debug.LogError("Can't parse generation path from identifier.");
								} else {
									var category = Nicify(matches.Groups[1].Value);
									var name = Nicify(matches.Groups[2].Value);
									var propertyName = whitespaceRegex.Replace(label.text, "");

									var textSource = TextSource.CreateInstance<TextSource>();
									var assetsPath = $"{Application.dataPath}";
									var folderPath = $"/Text/{category}/{name}";

									foreach (Lang lang in Enum.GetValues(typeof(Lang))) {
										Directory.CreateDirectory($"{assetsPath}{folderPath}");
										File.WriteAllText($"{assetsPath}{folderPath}/{propertyName}_{lang}.txt", $"{identifier.stringValue}.{propertyName}");
										AssetDatabase.Refresh();
										var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>($"Assets{folderPath}/{propertyName}_{lang}.txt");
										textSource.SetText(lang, textAsset);
									}

									AssetDatabase.CreateAsset(textSource, $"Assets{folderPath}/{propertyName}.asset");
									property.objectReferenceValue = textSource;
									EditorGUIUtility.PingObject(textSource);
									EditorGUIUtility.PingObject(textSource);

									static string Nicify(string str) {
										if (str.Length == 0) return str;
										var first = str[0];
										str = str.Remove(0, 1);
										str = $"{Char.ToUpper(first)}{str}";
										var i = 0;
										while (true && i++ < 100) {
											var underscore = str.IndexOf("_");
											if (underscore < 0) break;
											if (underscore >= str.Length - 1) {
												str = str.Remove(underscore, 1);
												break;
											}
											var old = str[underscore + 1];
											str = str.Remove(underscore, 2);
											str = str.Insert(underscore, Char.ToUpper(old).ToString());
										}
										return str;
									}
								}
							}
						}
					}
				}
			}
		}

	}
}
#endif


#if UNITY_EDITOR
namespace Editors {

	using System.Linq;
	using UnityEngine;
	using UnityEditor;
	using UnityEditorInternal;

	using static Muc.Editor.EditorUtil;
	using static Muc.Editor.PropertyUtil;

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
#endif