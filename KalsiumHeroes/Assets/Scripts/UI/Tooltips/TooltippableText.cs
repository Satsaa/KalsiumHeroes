
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using TMPro;
using UnityEngine.EventSystems;
using Muc.Extensions;

[RequireComponent(typeof(RectTransform))]
public class TooltippableText : TextMeshProUGUI, IPointerEnterHandler, IPointerExitHandler {

	[SerializeField] bool hovered;

	protected void Update() {
		if (hovered) {
			var linkIndex = TMP_TextUtilities.FindIntersectingLink(this, Input.mousePosition, null);
			if (linkIndex >= 0) {
				var linkInfo = textInfo.linkInfo[linkIndex];
				var src = linkInfo.GetLinkID();
				if (src.StartsWith("tt_")) {
					var id = src.Substring(3);

					var wordIndex = TMP_TextUtilities.FindIntersectingWord(this, Input.mousePosition, null);
					var wordInfo = textInfo.wordInfo[wordIndex];

					var rect = Rect.MinMaxRect(float.PositiveInfinity, float.PositiveInfinity, float.NegativeInfinity, float.NegativeInfinity);
					for (int i = wordInfo.firstCharacterIndex; i <= wordInfo.lastCharacterIndex; i++) {
						var charInfo = textInfo.characterInfo[i];
						var l = charInfo.vertex_TL.position.x; rect.xMin = Mathf.Min(rect.xMin, l);
						var t = charInfo.vertex_TL.position.y; rect.yMax = Mathf.Max(rect.yMax, t);
						var r = charInfo.vertex_BR.position.x; rect.xMax = Mathf.Max(rect.xMax, r);
						var b = charInfo.vertex_BR.position.y; rect.yMin = Mathf.Min(rect.yMin, b);
					}
					rect.center += transform.position.xy();
					if (rect.Contains(Input.mousePosition)) {
						Tooltips.instance.Show(id, gameObject, rect);
						if (Input.GetKeyDown(KeyCode.Mouse0)) {
							Tooltips.instance.Windowize();
						}
					}
				}
			}
		}
	}

	void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
		hovered = true;
	}

	void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
		hovered = false;
	}
}