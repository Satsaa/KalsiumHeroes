

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

		[SerializeField, Range(0, 1)] internal float renderScale = 1;
		[SerializeField] internal bool useScale = false;

		[SerializeField] internal Antialiasing antialiasing = Antialiasing.None;
		[SerializeField] internal RenderTextureFormat format = RenderTextureFormat.ARGB32;
		[SerializeField] internal DebthBits debthBits = DebthBits.Bits24Stencil;
		[SerializeField] internal bool enableMipMaps = false;
		[SerializeField] internal bool autoGenerateMips = false;
		[SerializeField] internal bool dynamicScaling = false;
		[SerializeField] internal FilterMode filterMode = FilterMode.Point;
		[SerializeField, Range(0, 16)] internal int anisoLevel = 0;

		[SerializeField] internal RenderTexture rt;
		[SerializeField] internal RenderObject renderObject;

		protected override void Awake() {
			base.Awake();
			if (Application.isPlaying) {
				rt = null;
				renderObject = null;
				CreateRenderObject();
				RefreshValues();
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
			RefreshValues();
		}


		protected virtual void CreateRenderObject() {
			renderObject = Instantiate(renderPrefab, RenderObjects.instance.transform);
			renderObject.camera.targetTexture = rt;
			RenderObjects.instance.AddObject(renderObject);
		}

		protected internal void RefreshValues() {
			if (!Application.isPlaying) return;

			var resolution = (rectTransform.rect.size * rectTransform.lossyScale).Mul(renderScale).Abs().RoundInt().Max(1);

			if (!rt) RecreateTexture();
			else if (resolution != rt.texelSize) RecreateTexture();
			else if (rt.useMipMap != enableMipMaps) RecreateTexture();
			else if (rt.autoGenerateMips != (autoGenerateMips && enableMipMaps)) RecreateTexture();
			else if (rt.antiAliasing != (int)antialiasing) RecreateTexture();
			else if (rt.useDynamicScale != dynamicScaling) RecreateTexture();
			else {
				rt.filterMode = filterMode;
				rt.anisoLevel = debthBits == DebthBits.None ? anisoLevel : 0;
			}
		}

		protected internal void RecreateTexture() {
			if (!Application.isPlaying) return;

			var resolution = (rectTransform.rect.size * rectTransform.lossyScale).Mul(renderScale).Abs().RoundInt().Max(1);

			var descriptor = new RenderTextureDescriptor(resolution.x, resolution.y, format, (int)debthBits);
			descriptor.useMipMap = enableMipMaps;
			descriptor.autoGenerateMips = autoGenerateMips && enableMipMaps;
			descriptor.msaaSamples = (int)antialiasing;
			descriptor.useDynamicScale = dynamicScaling;

			if (rt) rt.Release();
			texture = rt = new RenderTexture(descriptor);
			rt.name = $"{name} ({resolution.x}x{resolution.y})";

			rt.filterMode = filterMode;
			rt.anisoLevel = debthBits == DebthBits.None ? anisoLevel : 0;

			if (renderObject != null) {
				renderObject.camera.targetTexture = rt;
				renderObject.OnTextureChange();
			}
		}


		internal enum DebthBits {
			None = 0,
			Bits16 = 16,
			Bits24Stencil = 24,
			Bits32Stencil = 32
		}

		internal enum Antialiasing {
			None = 1,
			AA2 = 2,
			AA4 = 4,
			AA8 = 8
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
		SerializedProperty renderScale;
		SerializedProperty useScale;
		SerializedProperty antialiasing;
		SerializedProperty format;
		SerializedProperty debthBits;
		SerializedProperty enableMipMaps;
		SerializedProperty autoGenerateMips;
		SerializedProperty dynamicScaling;
		SerializedProperty filterMode;
		SerializedProperty anisoLevel;

		protected override void OnEnable() {
			base.OnEnable();
			renderPrefab = serializedObject.FindProperty(nameof(RenderImage.renderPrefab));
			renderScale = serializedObject.FindProperty(nameof(RenderImage.renderScale));
			useScale = serializedObject.FindProperty(nameof(RenderImage.useScale));
			antialiasing = serializedObject.FindProperty(nameof(RenderImage.antialiasing));
			format = serializedObject.FindProperty(nameof(RenderImage.format));
			debthBits = serializedObject.FindProperty(nameof(RenderImage.debthBits));
			enableMipMaps = serializedObject.FindProperty(nameof(RenderImage.enableMipMaps));
			autoGenerateMips = serializedObject.FindProperty(nameof(RenderImage.autoGenerateMips));
			dynamicScaling = serializedObject.FindProperty(nameof(RenderImage.dynamicScaling));
			filterMode = serializedObject.FindProperty(nameof(RenderImage.filterMode));
			anisoLevel = serializedObject.FindProperty(nameof(RenderImage.anisoLevel));
		}

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();
			serializedObject.Update();

			using (DisabledScope(Application.isPlaying)) EditorGUILayout.PropertyField(renderPrefab);

			EditorGUILayout.Space();

			EditorGUI.BeginChangeCheck();

			EditorGUILayout.PropertyField(renderScale);
			EditorGUILayout.PropertyField(useScale);
			EditorGUILayout.PropertyField(antialiasing);
			EditorGUILayout.PropertyField(format);
			EditorGUILayout.PropertyField(debthBits);
			EditorGUILayout.PropertyField(enableMipMaps);
			using (DisabledScope(!enableMipMaps.boolValue)) EditorGUILayout.PropertyField(autoGenerateMips);
			EditorGUILayout.PropertyField(dynamicScaling);
			EditorGUILayout.PropertyField(filterMode);
			using (DisabledScope(debthBits.intValue != 0)) EditorGUILayout.PropertyField(anisoLevel);

			serializedObject.ApplyModifiedProperties();
			if (EditorGUI.EndChangeCheck()) {
				foreach (RenderImage target in targets) {
					target.RefreshValues();
				}
			}
		}
	}
}
#endif