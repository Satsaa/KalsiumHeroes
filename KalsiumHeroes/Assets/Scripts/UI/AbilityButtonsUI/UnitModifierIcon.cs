using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Muc.Extensions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitModifierIcon : MonoBehaviour {

	[HideInInspector]
	public UnitModifier modifier;

	public TMP_Text displayName;
	public Image sprite;

	public void SetModifier(UnitModifier modifier) {
		this.modifier = modifier;
		displayName.text = modifier.data.displayName ?? modifier.data.identifier.Replace("_", " ");
		sprite.sprite = modifier.data.sprite;

		var modifierTtProvider = GetComponentInChildren<ModifierTooltipProvider>();
		/* if (modifierTtProvider)  */
		modifierTtProvider.modifier = modifier;
	}
}
