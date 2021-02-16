
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using TMPro;
using UnityEngine.EventSystems;
using Muc.Extensions;

public class ModifierListTooltipProvider : TooltipProvider {

	public Unit unit;
	public string id = "modifier_list";

	public RectTransform rectTransform => transform as RectTransform;

	protected override void OnHover() {

		if (!Input.GetKey(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse0)) {
			if (Tooltips.instance.Show(id, gameObject, rectTransform.rect)) {
				var tt = Tooltips.instance.Peek();
				if (tt is ModifierListTooltip mltt) {
					foreach (var modifier in unit.modifiers.Get<UnitModifier>()) {
						mltt.AddModifier(modifier);
					}
				}
			}
			if (Input.GetKeyDown(KeyCode.Mouse0)) {
				Tooltips.instance.Windowize();
			}
		} else {
			Tooltips.instance.Ping(id, gameObject, rectTransform.rect);
		}
	}

}