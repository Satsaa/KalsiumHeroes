using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Muc.Extensions;
using UnityEngine.UI;
using UnityEngine;

public class OverHeadUIModifier : UnitModifier, IOnLateUpdate {

	public new OverHeadUIModifierData data => (OverHeadUIModifierData)_data;
	public override Type dataType => typeof(OverHeadUIModifierData);

	[SerializeField, HideInInspector] OverHeadUI ui;

	[SerializeField, HideInInspector] float hpFullWidth;
	[SerializeField, HideInInspector] float energyFullWidth;

	protected override void OnCreate() {
		base.OnCreate();
		Debug.Assert(ui = container.GetComponentInChildren<OverHeadUI>());
		hpFullWidth = ui.hpRect.sizeDelta.x;
		energyFullWidth = ui.energyRect.sizeDelta.x;
	}

	void IOnLateUpdate.OnLateUpdate() {
		if (!ui) return;
		ui.transform.position = Camera.main.WorldToScreenPoint(master.gameObject.transform.position + data.wsOffset).Add(data.ssOffset);

		ui.hpRect.sizeDelta = ui.hpRect.sizeDelta.SetX(unit.data.health.value / unit.data.health.other * hpFullWidth);
		ui.hpText.text = $"{Mathf.Ceil(unit.data.health.value)}/{Mathf.Ceil(unit.data.health.other)}";

		ui.energyRect.sizeDelta = ui.energyRect.sizeDelta.SetX(unit.data.energy.value / (float)unit.data.energy.other * energyFullWidth);
		ui.energyText.text = $"{unit.data.energy.value}/{unit.data.energy.other}";
	}

}
