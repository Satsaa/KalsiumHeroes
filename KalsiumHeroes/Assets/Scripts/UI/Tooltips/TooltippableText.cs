
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
		RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, App.input.pointer, canvasCam, out var pointer);
		var linkIndex = TMP_TextUtilities.FindIntersectingLink(text, App.input.pointer, canvasCam);
		if (linkIndex >= 0) {
			var linkInfo = text.textInfo.linkInfo[linkIndex];
			var src = linkInfo.GetLinkID();
			if (src.StartsWith("tt_")) {
				var id = src.Substring(3);
				var rect = Rect.MinMaxRect(float.PositiveInfinity, float.PositiveInfinity, float.NegativeInfinity, float.NegativeInfinity);
				for (int i = linkInfo.linkTextfirstCharacterIndex; i <= linkInfo.linkTextfirstCharacterIndex + linkInfo.linkTextLength; i++) {
					var charInfo = text.textInfo.characterInfo[i];
					if (charInfo.isVisible) {
						var l = charInfo.topLeft.x; rect.xMin = Mathf.Min(rect.xMin, l);
						var t = charInfo.ascender; rect.yMax = Mathf.Max(rect.yMax, t);
						var r = charInfo.topRight.x; rect.xMax = Mathf.Max(rect.xMax, r);
						var b = charInfo.descender; rect.yMin = Mathf.Min(rect.yMin, b);
					}
				}
				if (!App.input.primary || App.input.primaryDown) {
					Tooltips.instance.Show(id, rectTransform, rect, canvasCam);
					if (App.input.primaryDown) {
						Tooltips.instance.InvokeOnCreatorClicked(rect);
					}
				} else {
					Tooltips.instance.Ping(id, rectTransform, rect);
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