
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using TMPro;
using UnityEngine.EventSystems;
using Muc.Extensions;
using UnityEngine.UI;

public class AbilityTooltip : UnitModifierTooltip {

	private Ability modifier => _modifier as Ability;

	public TMP_Text abilityTypeText;

	public TMP_Text maxChargesText;
	public TMP_Text curChargesText;

	public TMP_Text maxCooldownText;
	public TMP_Text curCooldownText;

	public TMP_Text energyCostText;

	public TMP_Text usesText;

	public void SetModifier(Ability modifier) {
		base.SetModifier(modifier);

		if (abilityTypeText) abilityTypeText.text = Enum.GetName(typeof(AbilityType), modifier.data.abilityType);

		if (maxChargesText) maxChargesText.text = modifier.data.charges.value.ToString();
		if (curChargesText) curChargesText.text = modifier.data.charges.other.ToString();

		if (maxCooldownText) maxCooldownText.text = modifier.data.cooldown.value.ToString();
		if (curCooldownText) curCooldownText.text = modifier.data.cooldown.other.ToString();

		if (energyCostText) energyCostText.text = modifier.data.energyCost.value.ToString();

		if (usesText) usesText.text = modifier.data.uses.enabled ? modifier.data.uses.value.ToString() : "∞";
	}

}