using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TextProvider {

	[Tooltip("When no " + nameof(TextSource) + " is provided, use this text.")]
	public string placeHolder;

	[Tooltip(nameof(TextSource) + " asset which is provides texts for multiple languages.")]
	public TextSource source;

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
	[CustomPropertyDrawer(typeof(TextProvider))]
	public class TextProviderDrawer : PropertyDrawer {

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

			if (!property.isExpanded) {
				using (PropertyScope(position, label, property, out label)) {

					var placeHolder = property.FindPropertyRelative(nameof(TextProvider.placeHolder));
					var source = property.FindPropertyRelative(nameof(TextProvider.source));
					EditorGUI.PropertyField(position, source.objectReferenceValue ? source : placeHolder, label);

					property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, GUIContent.none, true);

				}
			} else {
				EditorGUI.PropertyField(position, property, label, true);
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
								var category = ObjectNames.NicifyVariableName(matches.Groups[1].Value);
								var name = ObjectNames.NicifyVariableName(matches.Groups[2].Value);
								var file = whitespaceRegex.Replace(label.text, "");
								foreach (Lang lang in Enum.GetValues(typeof(Lang))) {
									var asset = new TextAsset($"{identifier}.{file}");
									var folderPath = $"{Application.dataPath}/Text/{category}/{name}/";
									var fileName = $"{file}_{lang}.txt";
									Directory.CreateDirectory(folderPath);
									AssetDatabase.CreateAsset(asset, $"{folderPath}{fileName}");
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
