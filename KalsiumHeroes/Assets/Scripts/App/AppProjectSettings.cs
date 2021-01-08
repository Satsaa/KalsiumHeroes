
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// <para>Store the settings for Muc that will be stored with the Unity Project.</para>
/// <para>Not usable for runtime data settings storage.</para>
/// </summary>
[FilePath("ProjectSettings/AppProjectSettings.asset", FilePathAttribute.Location.ProjectFolder)]
public class AppProjectSettings : ScriptableSingleton<AppProjectSettings> {

	[SerializeField]
	public TextSource defaultTextSource = null;

	void OnDisable() => Save();
	public void Save() => Save(true);
	public SerializedObject GetSerializedObject() => new SerializedObject(this);

}


namespace Editors {

	using System;
	using System.Linq;
	using System.Collections;
	using System.Collections.Generic;

	using UnityEngine;
	using UnityEditor;
	using UnityEngine.UIElements;
	using UnityEditorInternal;

	using Object = UnityEngine.Object;

	public class AppProjectSettingsProvider : SettingsProvider {

		SerializedObject serializedObject;

		SerializedProperty defaultTextSource;

		static AppProjectSettings t => AppProjectSettings.instance;

		private class Styles {
			public static readonly GUIContent defaultTextSource = EditorGUIUtility.TrTextContent($"Default {nameof(TextSource)}", "Default value for TextSources (editor only).");
		}

		public AppProjectSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords) { }

		public override void OnActivate(string searchContext, VisualElement rootElement) {
			AppProjectSettings.instance.Save();
			serializedObject = AppProjectSettings.instance.GetSerializedObject();
			defaultTextSource = serializedObject.FindProperty(nameof(defaultTextSource));
		}

		[SettingsProvider]
		public static SettingsProvider CreateSettingProvider() {
			return new AppProjectSettingsProvider("Project/Kalsium Heroes", SettingsScope.Project, GetSearchKeywordsFromGUIContentProperties<Styles>());
		}

		public override void OnGUI(string searchContext) {
			serializedObject.Update();

			using (new GUILayout.HorizontalScope()) {
				GUILayout.Space(10);
				using (new GUILayout.VerticalScope()) {
					GUILayout.Space(10);

					EditorGUIUtility.labelWidth += 100;

					EditorGUILayout.Space();

					defaultTextSource.objectReferenceValue = EditorGUILayout.ObjectField(Styles.defaultTextSource, defaultTextSource.objectReferenceValue, typeof(TextSource), false);

					EditorGUIUtility.labelWidth = 0;
				}
			}

			serializedObject.ApplyModifiedProperties();
		}

	}
}
#endif