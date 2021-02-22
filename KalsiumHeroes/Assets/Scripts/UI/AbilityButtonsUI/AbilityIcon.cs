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
	public Image maskSprite;

	public void SetAbility(Ability ability) {
		this.ability = ability;
		displayName.text = ability.data.displayName ?? ability.data.identifier.Replace("_", " ");
		cooldownText.text = "";
		chargeText.text = "";
		mask.gameObject.SetActive(true);
		sprite.enabled = false;
		sprite.sprite = maskSprite.sprite = ability.data.sprite;
	}
}
