using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Muc.Extensions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityIcon : MonoBehaviour {

	[HideInInspector]
	public Ability ability;

	public Button button;
	public TMP_Text displayName;
	public TMP_Text cooldownText;
	public TMP_Text chargeText;
	public TMP_Text energyText;

	public Image mask;
	public Image sprite;

	public void Deconstruct(out Ability ability, out Button abilityButton, out TMP_Text abilityText, out TMP_Text cooldownText, out TMP_Text chargeText, out TMP_Text energyText, out Image fgMask, out Image bgImage) {
		ability = this.ability;
		abilityButton = this.button;
		abilityText = this.displayName;
		cooldownText = this.cooldownText;
		energyText = this.energyText;
		chargeText = this.chargeText;
		fgMask = this.mask;
		bgImage = this.sprite;
	}

	public void SetAbility(Ability ability) {
		this.ability = ability;
		displayName.text = ability.data.displayName ?? ability.data.identifier.Replace("_", " ");
		cooldownText.text = "";
		chargeText.text = "";
		mask.gameObject.SetActive(true);
		sprite.enabled = false;
	}
}
