
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
		var linkIndex = TMP_TextUtilities.FindIntersectingLink(text, Input.mousePosition, null);
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
				rect.center += transform.position.xy();
				if (rect.Contains(Input.mousePosition)) {
					if (!Input.GetKey(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse0)) {
						Tooltips.instance.Show(id, gameObject, rect);
						if (Input.GetKeyDown(KeyCode.Mouse0)) {
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