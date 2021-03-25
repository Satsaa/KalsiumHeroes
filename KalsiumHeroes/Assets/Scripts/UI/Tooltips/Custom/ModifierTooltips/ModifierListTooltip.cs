
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using TMPro;
using UnityEngine.EventSystems;
using Muc.Extensions;

public class ModifierListTooltip : Tooltip {

	public UnitModifierIcon iconPrefab;

	public void AddModifier(UnitModifier unitModifier) {
		var icon = ObjectUtil.Instantiate(iconPrefab.gameObject, transform).GetComponent<UnitModifierIcon>();
		icon.SetModifier(unitModifier);
	}

}