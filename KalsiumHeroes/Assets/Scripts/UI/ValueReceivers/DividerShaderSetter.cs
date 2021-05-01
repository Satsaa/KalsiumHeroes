
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Extensions;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
public class DividerShaderSetter : ValueHooker<UnitData, Unit>, IOnTurnStart_Unit, IOnAbilityCastStart_Unit, IOnTakeDamage_Unit, IOnHeal_Unit, IOnAnimationEventEnd {

	[SerializeField]
	BarType barType;

	[SerializeField, Tooltip("This value is used for the maximum value if the attribute doesn't have one.")]
	float fallbackMaxValue = 100;

	[SerializeField, Tooltip("Set property named \"MaxValue\" to the maximum value of the attribute.")]
	bool setMaxValue = true;

	[SerializeField, Tooltip("Scale the property named \"WidthScale\" with .")]
	bool setWidthScale = true;

	[SerializeField, HideInInspector]
	float referenceWidth = -1;
	[SerializeField, HideInInspector]
	float referenceWidthScale = -1;

	// Called when dimensions of a RectTransform change
	protected void OnRectTransformDimensionsChange() {
		if (setWidthScale && Application.isPlaying) {
			var currentWidth = ((RectTransform)transform).rect.width;

			if (referenceWidth == -1) referenceWidth = currentWidth;
			if (referenceWidthScale == -1) referenceWidthScale = GetComponent<Graphic>().material.GetFloat("WidthScale");

			GetComponent<Graphic>().materialForRendering.SetFloat("WidthScale", currentWidth / referenceWidth * referenceWidthScale);
		}
	}

	protected override void ReceiveValue(UnitData data) {
		UpdateValue(data);
	}

	protected override void ReceiveValue(Unit target) {
		this.target = target;
		UpdateValue(target.data);
		Hook(target);
	}


	[SerializeField, HideInInspector]
	protected Unit target;

	protected void UpdateValue(UnitData data) {

		if (data && (setMaxValue || setWidthScale)) {

			var current = barType switch {
				BarType.Health => data.health.value,
				BarType.Defense => data.defense.value,
				BarType.Resistance => data.resistance.value,
				BarType.Speed => data.speed.value,
				BarType.Movement => data.movement.value,
				BarType.Energy => data.energy.value,
				BarType.EnergyRegen => data.energyRegen.value,
				BarType.MaxEnergy => data.energy.other,
				_ => throw new ArgumentOutOfRangeException("Not a valid enum value.", nameof(barType)),
			};

			var maxValue = barType switch {
				BarType.Health => data.health.other,
				BarType.Defense => fallbackMaxValue,
				BarType.Resistance => fallbackMaxValue,
				BarType.Speed => fallbackMaxValue,
				BarType.Movement => fallbackMaxValue,
				BarType.Energy => data.energy.other,
				BarType.EnergyRegen => fallbackMaxValue,
				BarType.MaxEnergy => data.energy.other,
				_ => throw new ArgumentOutOfRangeException("Not a valid enum value.", nameof(barType)),
			};
			maxValue = Mathf.Max(current, maxValue);

			var graphic = GetComponent<Graphic>();
			var material = graphic.materialForRendering;

			if (setMaxValue) material.SetFloat("MaxValue", maxValue);

			if (setWidthScale) {
				var currentWidth = ((RectTransform)transform).rect.width;

				if (referenceWidth == -1) referenceWidth = currentWidth;
				if (referenceWidthScale == -1) referenceWidthScale = GetComponent<Graphic>().material.GetFloat("WidthScale");

				material.SetFloat("WidthScale", currentWidth / referenceWidth * referenceWidthScale);
			}

		}

	}

	public void OnTurnStart() { if (barType == BarType.Energy) UpdateValue(target.data); }
	public void OnAbilityCastStart(Ability ability) { if (barType == BarType.Energy) UpdateValue(target.data); }
	public void OnTakeDamage(Modifier source, ref float damage, ref DamageType type) { if (barType == BarType.Health) UpdateValue(target.data); }
	public void OnHeal(ref float value) { if (barType == BarType.Health) UpdateValue(target.data); }
	public void OnAnimationEventEnd() => UpdateValue(target.data);

}