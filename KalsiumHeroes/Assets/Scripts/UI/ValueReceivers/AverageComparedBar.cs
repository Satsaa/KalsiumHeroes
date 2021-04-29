
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Extensions;
using UnityEngine.UI;

public class AverageComparedBar : ValueHooker<UnitData, Unit>, IOnTurnStart_Unit, IOnAbilityCastStart_Unit, IOnTakeDamage_Unit, IOnHeal_Unit, IOnAnimationEventEnd {

	[SerializeField] BarType barType;

	[SerializeField] TMPro.TMP_Text value;
	[SerializeField, Tooltip("Formatting string for the value text. {0} = current value, {1} = max value, {2} = average value, {3} = linear difference from average, {4} = percentage difference from average.")]
	string valueFormat = "{0}";

	[SerializeField] float maxValue;
	[SerializeField] Image baseFiller;
	[SerializeField] Image underFiller;
	[SerializeField] Image overFiller;

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

		if (data) {

			var current = barType switch {
				BarType.Health => data.health.value,
				BarType.Defense => data.defense.value,
				BarType.Resistance => data.resistance.value,
				BarType.Speed => data.speed.value,
				BarType.Movement => data.movement.value,
				BarType.Energy => data.energy.value,
				BarType.EnergyRegen => data.energyRegen.value,
				BarType.MaxEnergy => data.energy.other,
				_ => throw new ArgumentOutOfRangeException("Not a valid enum value", nameof(barType)),
			};

			var average = barType switch {
				BarType.Health => App.library.GetByType<UnitData>().Average(v => v.health.value),
				BarType.Defense => (float)App.library.GetByType<UnitData>().Average(v => (float)v.defense.value),
				BarType.Resistance => (float)App.library.GetByType<UnitData>().Average(v => (float)v.resistance.value),
				BarType.Speed => (float)App.library.GetByType<UnitData>().Average(v => (float)v.speed.value),
				BarType.Movement => (float)App.library.GetByType<UnitData>().Average(v => (float)v.movement.value),
				BarType.Energy => (float)App.library.GetByType<UnitData>().Average(v => (float)v.energy.value),
				BarType.EnergyRegen => (float)App.library.GetByType<UnitData>().Average(v => (float)v.energyRegen.value),
				BarType.MaxEnergy => (float)App.library.GetByType<UnitData>().Average(v => (float)v.energy.other),
				_ => throw new ArgumentOutOfRangeException("Not a valid enum value", nameof(barType)),
			};

			if (value) {
				var _0 = current;
				var _1 = maxValue;
				var _2 = average;
				var _3 = current - average;
				var _4 = (current - average) / average * 100; // Difference from average as percentage
				value.text = String.Format(valueFormat, _0, _1, _2, _3, _4);
			}

			if (baseFiller) {
				var fract = current / maxValue;
				baseFiller.gameObject.SetActive(fract > 0);
				RectTransform rt = baseFiller.rectTransform;
				rt.anchorMin = rt.anchorMin.SetX(0);
				rt.anchorMax = rt.anchorMax.SetX(fract);

			}

			var averageFract = average / maxValue;

			if (underFiller) {
				var fract = (average - current) / maxValue;
				underFiller.gameObject.SetActive(fract > 0);
				RectTransform rt = underFiller.rectTransform;
				rt.anchorMin = rt.anchorMin.SetX(averageFract - fract);
				rt.anchorMax = rt.anchorMax.SetX(averageFract);
			}

			if (overFiller) {
				var fract = (current - average) / maxValue;
				overFiller.gameObject.SetActive(fract > 0);
				RectTransform rt = overFiller.rectTransform;
				rt.anchorMin = rt.anchorMin.SetX(averageFract);
				rt.anchorMax = rt.anchorMax.SetX(averageFract + fract);
			}


		}

	}

	public void OnTurnStart() { if (barType == BarType.Energy) UpdateValue(target.data); }
	public void OnAbilityCastStart(Ability ability) { if (barType == BarType.Energy) UpdateValue(target.data); }
	public void OnTakeDamage(Modifier source, ref float damage, ref DamageType type) { if (barType == BarType.Health) UpdateValue(target.data); }
	public void OnHeal(ref float value) { if (barType == BarType.Health) UpdateValue(target.data); }
	public void OnAnimationEventEnd() => UpdateValue(target.data);

}