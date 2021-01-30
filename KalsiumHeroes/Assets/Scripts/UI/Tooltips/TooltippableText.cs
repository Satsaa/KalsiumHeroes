
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using TMPro;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class TooltippableText : TextMeshProUGUI, IPointerEnterHandler, IPointerExitHandler {

	[SerializeField] bool hovered;

	protected void Update() {
		if (hovered) {
			var link = TMP_TextUtilities.FindIntersectingLink(this, Input.mousePosition, null);
			if (link >= 0) {
				var lnkInfo = textInfo.linkInfo[link];
				var src = lnkInfo.GetLinkID();
				if (src.StartsWith("tt_")) {
					var id = src.Substring(3);
					Tooltips.instance.Show(id, gameObject);
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