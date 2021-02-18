
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using TMPro;
using UnityEngine.EventSystems;
using Muc.Extensions;
using UnityEngine.UI;

public class UnitModifierTooltip : ModifierTooltip {

	private UnitModifier modifier => _modifier as UnitModifier;

	public Image sprite;
	public TMP_Text description;
	public TMP_Text displayName;

	public void SetModifier(UnitModifier modifier) {
		base.SetModifier(modifier);

		if (sprite) sprite.sprite = modifier.data.sprite;
		if (description) description.text = modifier.data.description ?? "DESCRIPTION";
		if (displayName) displayName.text = modifier.data.displayName ?? "DISPLAYNAME";
	}

}