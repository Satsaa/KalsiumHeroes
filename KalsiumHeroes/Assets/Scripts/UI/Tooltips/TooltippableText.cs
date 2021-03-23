
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using TMPro;
using UnityEngine.EventSystems;
using Muc.Extensions;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TooltippableText : TooltipProvider {

	[SerializeField] TextMeshProUGUI text;

	new void OnValidate() {
#if UNITY_EDITOR // It appears this function doesnt exist in build
		base.OnValidate();
#endif
		if (!text) text = GetComponent<TextMeshProUGUI>();
	}

	new void Awake() {
		base.Awake();
		if (!text) text = GetComponent<TextMeshProUGUI>();
	}

	protected override void OnHover() {
		var linkIndex = TMP_TextUtilities.FindIntersectingLink(text, App.input.pointer, null);
		if (linkIndex >= 0) {
			var linkInfo = text.textInfo.linkInfo[linkIndex];
			var src = linkInfo.GetLinkID();
			if (src.StartsWith("tt_")) {
				var id = src.Substring(3);
				var rect = Rect.MinMaxRect(float.PositiveInfinity, float.PositiveInfinity, float.NegativeInfinity, float.NegativeInfinity);
				for (int i = linkInfo.linkTextfirstCharacterIndex; i <= linkInfo.linkTextfirstCharacterIndex + linkInfo.linkTextLength; i++) {
					var charInfo = text.textInfo.characterInfo[i];
					if (charInfo.isVisible) {
						var l = charInfo.vertex_TL.position.x; rect.xMin = Mathf.Min(rect.xMin, l);
						var t = charInfo.vertex_TL.position.y; rect.yMax = Mathf.Max(rect.yMax, t);
						var r = charInfo.vertex_BR.position.x; rect.xMax = Mathf.Max(rect.xMax, r);
						var b = charInfo.vertex_BR.position.y; rect.yMin = Mathf.Min(rect.yMin, b);
					}
				}
				rect = rect.Scale(transform.lossyScale);
				rect.center *= transform.lossyScale;
				rect.center += transform.position.xy();
				if (rect.Contains(App.input.pointer)) {
					if (!App.input.primary || App.input.primaryDown) {
						Tooltips.instance.Show(id, gameObject, rect);
						if (App.input.primaryDown) {
							Tooltips.instance.Windowize();
						}
					} else {
						Tooltips.instance.Ping(id, gameObject, rect);
					}
				}
			}
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
	[CustomEditor(typeof(TooltippableText), true)]
	public class TooltippableTextEditor : Editor {

		TooltippableText t => (TooltippableText)target;

		// SerializedProperty property;

		void OnEnable() {
			// property = serializedObject.FindProperty(nameof(property));
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();

			ScriptField(serializedObject);

			DrawPropertiesExcluding(serializedObject,
				script,
				$"<{nameof(TooltipProvider.id)}>k__BackingField"
			);

			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif