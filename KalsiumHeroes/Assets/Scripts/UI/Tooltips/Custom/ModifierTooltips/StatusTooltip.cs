
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using TMPro;
using UnityEngine.EventSystems;
using Muc.Extensions;
using UnityEngine.UI;

public class StatusTooltip : UnitModifierTooltip {

	private Status modifier => _modifier as Status;

	public Toggle dispellableToggle;
	public Toggle positiveToggle;
	public Toggle hiddenToggle;

	public TMP_Text tickModeText;
	public TMP_Text maxTicksText;
	public TMP_Text curTicksText;


	public void SetModifier(Status modifier) {
		base.SetModifier(modifier);

		if (dispellableToggle) dispellableToggle.isOn = modifier.data.dispellable.value;
		if (positiveToggle) positiveToggle.isOn = modifier.data.positive.value;
		if (hiddenToggle) hiddenToggle.isOn = modifier.data.hidden.value;

		if (tickModeText) tickModeText.text = Enum.GetName(typeof(TickMode), modifier.data.tickMode);
		if (maxTicksText) maxTicksText.text = modifier.data.ticks.value.ToString();
		if (curTicksText) curTicksText.text = modifier.data.ticks.other.ToString();
	}

}