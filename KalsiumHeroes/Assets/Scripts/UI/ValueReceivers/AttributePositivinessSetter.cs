
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Extensions;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
public class AttributePositivinessSetter : ValueHooker<UnitData, Unit>, IOnTurnStart_Unit, IOnAbilityCastStart_Unit, IOnTakeDamage_Unit, IOnHeal_Unit, IOnAnimationEventEnd {

	[SerializeField] BarType barType;

	[SerializeField] Color neutralColor = Color.white;
	[SerializeField] Color positiveColor = Color.green;
	[SerializeField] Color negativeColor = Color.red;

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

			var raw = barType switch {
				BarType.Health => data.health.rawValue,
				BarType.Defense => data.defense.rawValue,
				BarType.Resistance => data.resistance.rawValue,
				BarType.Speed => data.speed.rawValue,
				BarType.Movement => data.movement.rawValue,
				BarType.Energy => data.energy.rawValue,
				BarType.EnergyRegen => data.energyRegen.rawValue,
				BarType.MaxEnergy => data.energy.rawOther,
				_ => throw new ArgumentOutOfRangeException("Not a valid enum value", nameof(barType)),
			};

			var sign = barType switch {
				BarType.Health => current.CompareTo(raw),
				BarType.Defense => current.CompareTo(raw),
				BarType.Resistance => current.CompareTo(raw),
				BarType.Speed => current.CompareTo(raw),
				BarType.Movement => current.CompareTo(raw),
				BarType.Energy => current.CompareTo(raw),
				BarType.EnergyRegen => current.CompareTo(raw),
				BarType.MaxEnergy => current.CompareTo(raw),
				_ => throw new ArgumentOutOfRangeException("Not a valid enum value", nameof(barType)),
			};

			GetComponent<Graphic>().color = sign == -1 ? negativeColor : sign == 1 ? positiveColor : neutralColor;

		}

	}

	public void OnTurnStart() { if (barType == BarType.Energy) UpdateValue(target.data); }
	public void OnAbilityCastStart(Ability ability) { if (barType == BarType.Energy) UpdateValue(target.data); }
	public void OnTakeDamage(Modifier source, ref float damage, ref DamageType type) { if (barType == BarType.Health) UpdateValue(target.data); }
	public void OnHeal(ref float value) { if (barType == BarType.Health) UpdateValue(target.data); }
	public void OnAnimationEventEnd() => UpdateValue(target.data);

}