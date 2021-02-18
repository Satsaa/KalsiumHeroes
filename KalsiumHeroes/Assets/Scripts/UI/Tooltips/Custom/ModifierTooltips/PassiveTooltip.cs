
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using TMPro;
using UnityEngine.EventSystems;
using Muc.Extensions;
using UnityEngine.UI;

public class PassiveTooltip : UnitModifierTooltip {

	private Passive modifier => _modifier as Passive;

	public void SetModifier(Passive modifier) {
		base.SetModifier(modifier);
	}

}