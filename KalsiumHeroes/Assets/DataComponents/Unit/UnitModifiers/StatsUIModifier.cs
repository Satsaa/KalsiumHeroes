using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Muc.Extensions;
using UnityEngine.UI;
using UnityEngine;

public class StatsUIModifier : UnitModifier, IOnLateUpdate {

	public new StatsUIModifierData data => (StatsUIModifierData)_data;
	public override Type dataType => typeof(StatsUIModifierData);

	[SerializeField, HideInInspector] Camera cam;
	[SerializeField, HideInInspector] StatsUI ui;

	[SerializeField, HideInInspector] float hpFullWidth;
	[SerializeField, HideInInspector] float energyFullWidth;

	void IOnLateUpdate.OnLateUpdate() {
		if (!ui) return;
		ui.transform.position = cam.WorldToScreenPoint(master.gameObject.transform.position + data.wsOffset).Add(data.ssOffset);

		ui.hpRect.sizeDelta = ui.hpRect.sizeDelta.SetX(unit.data.health.value / unit.data.health.other * hpFullWidth);
		ui.hpText.text = $"{Mathf.Ceil(unit.data.health.value)}/{Mathf.Ceil(unit.data.health.other)}";

		ui.energyRect.sizeDelta = ui.energyRect.sizeDelta.SetX(unit.data.energy.value / (float)unit.data.energy.other * energyFullWidth);
		ui.energyText.text = $"{unit.data.energy.value}/{unit.data.energy.other}";
	}

	protected override void OnCreate() {
		base.OnCreate();
		Debug.Assert(ui = container.GetComponentInChildren<StatsUI>());
		Debug.Assert(cam = Camera.main);
		hpFullWidth = ui.hpRect.sizeDelta.x;
		energyFullWidth = ui.energyRect.sizeDelta.x;
	}

}
