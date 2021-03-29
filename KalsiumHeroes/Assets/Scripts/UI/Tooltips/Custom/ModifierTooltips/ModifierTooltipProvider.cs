
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using TMPro;
using UnityEngine.EventSystems;
using Muc.Extensions;

public class ModifierTooltipProvider : TooltipProvider {

	public Modifier modifier;

	public string abilityTooltip = "ability_info";
	public string passiveTooltip = "passive_info";
	public string statusTooltip = "status_info";
	public string unitModifierTooltip = "unitmodifier_info";
	public string modifierTooltip = "modifier_info";


	protected override void OnHover() {

		var id = modifier switch {
			Ability _ => abilityTooltip,
			Passive _ => passiveTooltip,
			Status _ => statusTooltip,
			UnitModifier _ => unitModifierTooltip,
			Modifier _ => modifierTooltip,
		};

		if (!App.input.primary || App.input.primaryDown) {
			Tooltips.instance.Show(id, rectTransform, rectTransform.rect, canvasCam, Initialize);
			if (App.input.primaryDown) {
				Tooltips.instance.InvokeOnCreatorClicked(rectTransform.rect);
			}
		} else {
			Tooltips.instance.Ping(id, rectTransform, rectTransform.rect);
		}
	}

	private void Initialize(Tooltip _tooltip) {
		switch (_tooltip) {

			case AbilityTooltip ability:
				ability.SetModifier(modifier as Ability);
				break;

			case PassiveTooltip passive:
				passive.SetModifier(modifier as Passive);
				break;

			case StatusTooltip status:
				status.SetModifier(modifier as Status);
				break;

			case UnitModifierTooltip unitModifier:
				unitModifier.SetModifier(modifier as UnitModifier);
				break;

			case ModifierTooltip modifierTooltip:
				modifierTooltip.SetModifier(modifier);
				break;

			default:
				Debug.LogWarning($"Unexpected {nameof(Tooltip)} type {_tooltip.GetType().Name}.");
				break;
		}
	}
}