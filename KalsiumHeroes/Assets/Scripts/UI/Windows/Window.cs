
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Extensions;
using UnityEngine.EventSystems;
using Muc.Components.Extended;

public class Window : ExtendedUIBehaviour {

	public bool dragging { get; set; }

	// Hidden
	public WindowContent content;
	public WindowToolbar toolbar;
	public WindowScrollRect scrollRect;
	public List<WindowResizer> resizers;

	// Shown
	[Tooltip("When resize is disabled you can fully remove the scroll rect and use a plain WindowContent")]
	public bool allowResize = true;
	[Min(30)] public float minWidth = 30;
	[Min(15)] public float minHeight = 15;
	public RectTransform contentFallback;

	[Flags]
	public enum Edge {
		Left = 1,
		Right = 1 << 1,
		Bottom = 1 << 2,
		Top = 1 << 3,
	}

	new protected void Awake() {
		base.Awake();
		content = GetComponentInChildren<WindowContent>(true);
		toolbar = GetComponentInChildren<WindowToolbar>(true);
		scrollRect = GetComponentInChildren<WindowScrollRect>(true);
		GetComponentsInChildren<WindowResizer>(true, resizers);
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
		if (!allowResize || !content) return;
		content.LateUpdate();
		var toolBarRect = toolbar.rectTransform;
		var oldRect = rectTransform.rect;
		var rect = content.contentRect.rect;
		rectTransform.sizeDelta = new Vector2(
			rect.width,
			rect.height + toolBarRect.rect.height
		);
		if (keepPosition) {
			var newRect = rectTransform.rect;
			var diff = new Vector3(oldRect.xMin, oldRect.yMax) - new Vector3(newRect.xMin, newRect.yMax);
			transform.localPosition += diff;
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
		SerializedProperty contentFallback;

		void OnEnable() {
			allowResize = serializedObject.FindProperty(nameof(Window.allowResize));
			minWidth = serializedObject.FindProperty(nameof(Window.minWidth));
			minHeight = serializedObject.FindProperty(nameof(Window.minHeight));
			contentFallback = serializedObject.FindProperty(nameof(Window.contentFallback));
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();

			ScriptField(serializedObject);

			EditorGUILayout.PropertyField(allowResize);
			if (allowResize.boolValue) {
				EditorGUILayout.PropertyField(minWidth);
				EditorGUILayout.PropertyField(minHeight);
				EditorGUILayout.PropertyField(contentFallback);
			}


			DrawPropertiesExcluding(serializedObject,
				script,
				nameof(Window.content),
				nameof(Window.resizers),
				nameof(Window.toolbar),
				nameof(Window.scrollRect),
				allowResize.name,
				minWidth.name,
				minHeight.name,
				contentFallback.name
			);

			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif