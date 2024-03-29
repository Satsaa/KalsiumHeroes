
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Extensions;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Rendering;
using Muc.Components.Extended;

[ExecuteAlways]
[DefaultExecutionOrder(-1000)]
[RequireComponent(typeof(RectTransform))]
public class WindowContent : ExtendedUIBehaviour {

	public Window window;

	public ScrollRect scrollRect;
	public RectTransform contentRect;
	public RectTransform contentParent;

	public void LateUpdate() {

		if (!window || !window.allowResize || !contentParent || !contentRect || !scrollRect) return;
		if (contentParent.childCount == 0) return;

		var rect = Rect.MinMaxRect(float.PositiveInfinity, float.PositiveInfinity, float.NegativeInfinity, float.NegativeInfinity);
		foreach (RectTransform child in contentParent) {
			var pos = child.localPosition;
			rect.min = new Vector2(
				Mathf.Min(rect.min.x, pos.x + child.rect.min.x),
				Mathf.Min(rect.min.y, -pos.y + -child.rect.max.y)
			);
			rect.max = new Vector2(
				Mathf.Max(rect.max.x, pos.x + child.rect.max.x),
				Mathf.Max(rect.max.y, -pos.y + -child.rect.min.y)
			);
		}

		contentRect.sizeDelta = new Vector2(rect.width, rect.height);

		contentParent.localPosition = new Vector3(
			(rect.width - scrollRect.viewport.rect.width) * (-(scrollRect.horizontalScrollbar.isActiveAndEnabled ? scrollRect.horizontalScrollbar.value : 0)) - rect.xMin,
			(rect.height - scrollRect.viewport.rect.height) * (scrollRect.verticalScrollbar.isActiveAndEnabled ? 2 - scrollRect.verticalScrollbar.value : 1) - rect.height + rect.yMin + scrollRect.viewport.rect.height,
			contentParent.localPosition.z
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
	[CustomEditor(typeof(WindowContent), true)]
	public class WindowContentEditor : Editor {

		WindowContent t => (WindowContent)target;

		SerializedProperty window;
		SerializedProperty scrollRect;
		SerializedProperty contentRect;
		SerializedProperty contentParent;

		void OnEnable() {
			window = serializedObject.FindProperty(nameof(WindowContent.window));
			scrollRect = serializedObject.FindProperty(nameof(WindowContent.scrollRect));
			contentRect = serializedObject.FindProperty(nameof(WindowContent.contentRect));
			contentParent = serializedObject.FindProperty(nameof(WindowContent.contentParent));
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();

			ScriptField(serializedObject);

			EditorGUILayout.PropertyField(window);
			var windowValue = window.objectReferenceValue as Window;
			using (DisabledScope(windowValue && windowValue.allowResize)) {
				EditorGUILayout.PropertyField(scrollRect);
				EditorGUILayout.PropertyField(contentRect);
				EditorGUILayout.PropertyField(contentParent);
			}

			DrawPropertiesExcluding(serializedObject,
				script,
				window.name,
				scrollRect.name,
				contentRect.name,
				contentParent.name
			);

			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif