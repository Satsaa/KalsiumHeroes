
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Extensions;
using UnityEngine.UI;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class AttributeSetter : ValueHooker<UnitData, Unit>, IOnTurnStart_Unit, IOnAbilityCastStart_Unit, IOnTakeDamage_Unit, IOnHeal_Unit, IOnAnimationEventEnd {

	[SerializeField] BarType barType;

	[SerializeField, Tooltip("Formatting string for the text. {0} = current value, {1} = max value, {2} = percent full.")]
	string format = "{0}/{1}";

	[SerializeField, Tooltip("This value is used for the maximum value if the attribute doesn't have one.")]
	float fallbackMaxValue = 100;

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

			var maxValue = barType switch {
				BarType.Health => data.health.other,
				BarType.Defense => fallbackMaxValue,
				BarType.Resistance => fallbackMaxValue,
				BarType.Speed => fallbackMaxValue,
				BarType.Movement => fallbackMaxValue,
				BarType.Energy => data.energy.other,
				BarType.EnergyRegen => fallbackMaxValue,
				BarType.MaxEnergy => data.energy.other,
				_ => throw new ArgumentOutOfRangeException("Not a valid enum value", nameof(barType)),
			};

			GetComponent<TMPro.TMP_Text>().text = String.Format(format, current, maxValue, current / maxValue * 100);

		}

	}

	public void OnTurnStart() { if (barType == BarType.Energy) UpdateValue(target.data); }
	public void OnAbilityCastStart(Ability ability) { if (barType == BarType.Energy) UpdateValue(target.data); }
	public void OnTakeDamage(Modifier source, ref float damage, ref DamageType type) { if (barType == BarType.Health) UpdateValue(target.data); }
	public void OnHeal(ref float value) { if (barType == BarType.Health) UpdateValue(target.data); }
	public void OnAnimationEventEnd() => UpdateValue(target.data);

}