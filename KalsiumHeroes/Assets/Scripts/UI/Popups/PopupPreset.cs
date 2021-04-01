using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Muc.Data;
using UnityEngine.UI;
using System;
using TMPro;
using Object = UnityEngine.Object;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = nameof(PopupPreset), menuName = "KalsiumHeroes/" + nameof(PopupPreset))]
public class PopupPreset : ScriptableObject {

	[SerializeField] internal Popup popupPrefab;
	[SerializeField] internal PopupOption optionPrefab;

	[SerializeField] TextSource baseTitle;
	[SerializeField] TextSource baseMessage;
	[SerializeField] TextSource baseOptionText;

	public struct Option {
		public string text;
		public Action action;
		public PopupOption.Flags key;

		public Option(string text, Action action, PopupOption.Flags key = 0) {
			this.text = text;
			this.action = action;
			this.key = key;
		}
	}

	protected virtual void DoTitle(Popup msgBox, string title) {
		msgBox.SetTitle(title);
	}

	protected virtual void DoMessage(Popup msgBox, string message) {
		msgBox.SetMessage(message);
	}

	protected virtual void DoCustom(Popup msgBox) {

	}

	protected virtual void DoOption(Popup msgBox, Option option) {
		var opt = msgBox.AddOption(optionPrefab, option.action);
		opt.SetText(option.text);
		opt.flags = option.key;
	}


	public Popup Show(string message) => Show(null, message);
	public Popup Show(string title, string message) {
		return Show(title, message, new Option(baseOptionText, null, PopupOption.Flags.Cancel | PopupOption.Flags.Default));
	}

	public Popup Show(string message, params Option[] options) => Show(null, message, options);
	public Popup Show(string title, string message, params Option[] options) {
		if (popupPrefab == null || optionPrefab == null) {
			Debug.LogError($"{nameof(popupPrefab)} or {nameof(optionPrefab)} is not set. Alternatively a reference may be broken and you need to reassign them in editor. To do that double click this message and press the reassign button.", this);
			if (popupPrefab == null) throw new ArgumentNullException($"Argument cannot be null.", nameof(popupPrefab));
			if (optionPrefab == null) throw new ArgumentNullException($"Argument cannot be null.", nameof(optionPrefab));
		}
		var msgBox = Instantiate(popupPrefab);
		msgBox.gameObject.transform.SetParent(Popups.rectTransform);
		DoTitle(msgBox, title);
		DoMessage(msgBox, message);
		DoCustom(msgBox);
		foreach (var option in options) {
			DoOption(msgBox, option);
		}
		return msgBox;
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
	[CustomEditor(typeof(PopupPreset), true)]
	public class PopupPresetEditor : Editor {

		PopupPreset t => (PopupPreset)target;

		SerializedProperty popupPrefab;
		SerializedProperty optionPrefab;

		void OnEnable() {
			popupPrefab = serializedObject.FindProperty(nameof(PopupPreset.popupPrefab));
			optionPrefab = serializedObject.FindProperty(nameof(PopupPreset.optionPrefab));
		}

		public override void OnInspectorGUI() {
			DrawDefaultInspector();

			using (DisabledScope(Application.isPlaying)) {
				if (GUILayout.Button($"{(Application.isPlaying ? "Disabled in play. " : "")}Reassign fields (may fix missing ref exception)")) {
					serializedObject.Update();

					var oldPops = new Popup[targets.Length];
					var oldOpts = new PopupOption[targets.Length];

					for (int i = 0; i < targets.Length; i++) {
						var target = (PopupPreset)targets[i];
						oldPops[i] = target.popupPrefab;
						oldOpts[i] = target.optionPrefab;
						target.popupPrefab = null;
						target.optionPrefab = null;
					}

					serializedObject.ApplyModifiedProperties();
					serializedObject.Update();

					for (int i = 0; i < targets.Length; i++) {
						var target = (PopupPreset)targets[i];
						target.popupPrefab = oldPops[i];
						target.optionPrefab = oldOpts[i];
					}

					serializedObject.ApplyModifiedProperties();
				}
			}

		}
	}
}
#endif