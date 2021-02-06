
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Extensions;

public class Window : MonoBehaviour {

	[HideInInspector] public bool dragging = false;
	public WindowContent content;
	public List<WindowResizer> resizers;
	public WindowToolbar toolbar;

	[Flags]
	public enum Edge {
		Left = 1,
		Right = 2,
		Bottom = 4,
		Top = 8,
	}

	public RectTransform rectTransform => (RectTransform)transform;

	void Reset() {
		content = GetComponentInChildren<WindowContent>(true);
		GetComponentsInChildren<WindowResizer>(true, resizers);
		toolbar = GetComponentInChildren<WindowToolbar>(true);
	}

	protected void Start() {
		FitSize();
	}

	public void Destroy() {
		Destroy(gameObject);
	}

	void OnDestroy() {
		Destroy(gameObject);
	}

	public void FitSize() {
		content.LateUpdate();
		var contentRect = (content.contentRect.transform as RectTransform).rect;
		rectTransform.sizeDelta = new Vector2(
			contentRect.width,
			contentRect.height + toolbar.rectTransform.rect.height
		);
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

		// SerializedProperty property;

		void OnEnable() {
			// property = serializedObject.FindProperty(nameof(property));
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();

			DrawDefaultInspector();
			if (GUILayout.Button("FitSize")) {
				t.FitSize();
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif