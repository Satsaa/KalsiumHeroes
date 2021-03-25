
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using TMPro;
using UnityEngine.EventSystems;
using Muc.Extensions;

public class ModifierListTooltipProvider : TooltipProvider, IContainerComponent {

	public Unit unit;

	protected override void InitializeTooltip(Tooltip tooltip) {
		if (tooltip is ModifierListTooltip mlTooltip) {
			foreach (var modifier in unit.modifiers.Get<UnitModifier>()) {
				mlTooltip.AddModifier(modifier);
			}
		} else {
			Debug.LogWarning($"Unexpected {nameof(Tooltip)} type.");
		}
	}

	void IContainerComponent.SetMaster(Master master) => unit = (Unit)master;
}