using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Muc.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class AbilityIcon : MonoBehaviour {

	[HideInInspector]
	public Ability ability;

	public Button abilityButton;
	public Text abilityText;
	public Text cooldownText;
	public Text chargeText;

	public Image fgImage;
	public Image bgImage;

	public void Deconstruct(out Ability ability, out Button abilityButton, out Text abilityText, out Text cooldownText, out Text chargeText, out Image fgImage, out Image bgImage) {
		ability = this.ability;
		abilityButton = this.abilityButton;
		abilityText = this.abilityText;
		cooldownText = this.cooldownText;
		chargeText = this.chargeText;
		fgImage = this.fgImage;
		bgImage = this.bgImage;
	}

}
