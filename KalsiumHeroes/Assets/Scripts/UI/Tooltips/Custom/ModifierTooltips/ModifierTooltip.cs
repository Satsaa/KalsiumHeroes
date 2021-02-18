
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using TMPro;
using UnityEngine.EventSystems;
using Muc.Extensions;
using UnityEngine.UI;

public class ModifierTooltip : Tooltip {

	[HideInInspector] public Modifier _modifier;

	public TMP_Text typeName;
	public TMP_Text modifierId;

	public void SetModifier(Modifier modifier) {
		_modifier = modifier;
		if (typeName) typeName.text = modifier.GetType().Name;
		if (modifierId) modifierId.text = modifier.data.identifier;
	}

}