
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Extensions;
using UnityEngine.EventSystems;

public class Window : UIBehaviour {

	public RectTransform rectTransform => (RectTransform)transform;

	public bool dragging { get; set; }

	// Hidden
	public WindowContent content;
	public List<WindowResizer> resizers;
	public WindowToolbar toolbar;
	public WindowScrollRect scrollRect;

	// Shown
	public bool allowResize = true;
	[Min(30)] public float minWidth = 30;
	[Min(15)] public float minHeight = 15;

	[Flags]
	public enum Edge {
		Left = 1,
		Right = 2,
		Bottom = 4,
		Top = 8,
	}

	new protected void Awake() {
		base.Awake();
		content = GetComponentInChildren<WindowContent>(true);
		GetComponentsInChildren<WindowResizer>(true, resizers);
		toolbar = GetComponentInChildren<WindowToolbar>(true);
		scrollRect = GetComponentInChildren<WindowScrollRect>(true);
	}

	new protected void Start() {
		base.Start();
		FitSize();
	}

	public void Destroy() {
		Destroy(gameObject);
	}

	new protected void OnDestroy() {
		base.OnDestroy();
		Destroy(gameObject);
	}

	public void FitSize(bool keepPosition = false) {
		content.LateUpdate();
		var toolBarRect = (RectTransform)toolbar.transform;
		var oldPos = toolBarRect.ScreenRect();
		var rect = content.rectTransform.rect;
		rectTransform.sizeDelta = new Vector2(
			rect.width,
			rect.height + toolBarRect.rect.height
		);
		if (keepPosition) {
			var newPos = toolBarRect.ScreenRect();
			var diff = new Vector3(newPos.xMin, newPos.yMax) - new Vector3(oldPos.xMin, oldPos.yMax);
			transform.Translate(-diff);
		}
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
	[CustomEditor(typeof(Window), true)]
	public class WindowEditor : Editor {

		Window t => (Window)target;

		SerializedProperty allowResize;
		SerializedProperty minWidth;
		SerializedProperty minHeight;

		void OnEnable() {
			allowResize = serializedObject.FindProperty(nameof(Window.allowResize));
			minWidth = serializedObject.FindProperty(nameof(Window.minWidth));
			minHeight = serializedObject.FindProperty(nameof(Window.minHeight));
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();

			ScriptField(serializedObject);

			EditorGUILayout.PropertyField(allowResize);
			if (allowResize.boolValue) {
				EditorGUILayout.PropertyField(minWidth);
				EditorGUILayout.PropertyField(minHeight);
			}


			DrawPropertiesExcluding(serializedObject,
				script,
				nameof(Window.content),
				nameof(Window.resizers),
				nameof(Window.toolbar),
				nameof(Window.scrollRect),
				allowResize.name,
				minWidth.name,
				minHeight.name
			);

			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif