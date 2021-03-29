

namespace Muc.Systems.RenderImages {

	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using Muc.Extensions;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.UI;

#if (MUC_HIDE_COMPONENTS || MUC_HIDE_SYSTEM_COMPONENTS)
	[AddComponentMenu("")]
#else
	[AddComponentMenu("MyUnityCollection/" + nameof(Muc.Systems.RenderImages) + "/" + nameof(RenderImage))]
#endif
	public class RenderImage : RawImage {

		[SerializeField] internal RenderObject renderPrefab;
		[SerializeField] internal RenderTextureFormat format = RenderTextureFormat.ARGB32;
		[SerializeField] internal int debthBits = 16;

		[SerializeField] internal RenderTexture rt;
		[SerializeField] internal RenderObject renderObject;

		protected override void Awake() {
			base.Awake();
			if (Application.isPlaying) {
				rt = null;
				renderObject = null;
				CreateRenderObject();
				RefreshTexture();
			}
		}

		protected override void Start() {
			base.Start();
		}

		protected override void OnEnable() {
			base.OnEnable();
			if (Application.isPlaying) {
				if (renderObject) renderObject.gameObject.SetActive(true);
			}
		}

		protected override void OnDisable() {
			base.OnDisable();
			if (Application.isPlaying) {
				if (renderObject) renderObject.gameObject.SetActive(false);
			}
		}

		protected override void OnDestroy() {
			base.OnDestroy();
			if (Application.isPlaying) {
				if (rt) {
					rt.Release();
					texture = null;
				}
				if (renderObject) {
					Destroy(renderObject.gameObject);
				}
			}
		}

		protected override void OnRectTransformDimensionsChange() {
			base.OnRectTransformDimensionsChange();
			if (rt == null) return;
			RefreshTexture();
		}


		protected virtual void CreateRenderObject() {
			renderObject = Instantiate(renderPrefab, RenderObjects.instance.transform);
			renderObject.camera.targetTexture = rt;
			RenderObjects.instance.AddObject(renderObject);
		}

		protected virtual void RefreshTexture() {
			if (!Application.isPlaying) return;
			var doRefresh = false;

			var resolution = (rectTransform.rect.size * rectTransform.lossyScale).FloorInt();
			if (!rt || resolution != rt.texelSize) doRefresh = true;

			if (doRefresh) {
				if (rt) rt.Release();
				rt = new RenderTexture(resolution.x, resolution.y, debthBits, format);
				texture = rt;
				if (renderObject != null) {
					renderObject.camera.targetTexture = rt;
					renderObject.rendered = false;
				}
			}
		}

	}

}

#if UNITY_EDITOR
namespace Muc.Systems.RenderImages {

	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using UnityEditor.UI;
	using Object = UnityEngine.Object;
	using static Muc.Editor.PropertyUtil;
	using static Muc.Editor.EditorUtil;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(RenderImage), true)]
	internal class RenderImageEditor : RawImageEditor {

		RenderImage t => (RenderImage)target;

		SerializedProperty renderPrefab;

		protected override void OnEnable() {
			base.OnEnable();
			renderPrefab = serializedObject.FindProperty(nameof(RenderImage.renderPrefab));
		}

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();
			serializedObject.Update();

			EditorGUILayout.Space();

			EditorGUILayout.PropertyField(renderPrefab);

			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif