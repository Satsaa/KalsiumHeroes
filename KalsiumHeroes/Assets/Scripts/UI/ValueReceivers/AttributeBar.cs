
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Extensions;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AttributeBar : ValueHooker<UnitData, Unit>, IOnTurnStart_Unit, IOnAbilityCastStart_Unit, IOnTakeDamage_Unit, IOnHeal_Unit, IOnAnimationEventEnd {

	[SerializeField] BarType barType;
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
			maxValue = Mathf.Max(current, maxValue);


			var fract = current / maxValue;

			var image = GetComponent<Image>();

			image.gameObject.SetActive(fract > 0);
			RectTransform rt = image.rectTransform;

			var correction = 0f;
			if (image.type is Image.Type.Sliced or Image.Type.Tiled && image.hasBorder) {
				var pixels = (image.sprite.border.x * (1 - fract) * (1 / image.pixelsPerUnitMultiplier));
				correction += pixels / (rt.rect.width * ((rt.anchorMax.x - rt.anchorMin.x) * -1 + 2));
				correction *= 2;
			}
			rt.anchorMin = rt.anchorMin.SetX(0);
			rt.anchorMax = rt.anchorMax.SetX(fract + correction);

		}

	}

	public void OnTurnStart() { if (barType == BarType.Energy) UpdateValue(target.data); }
	public void OnAbilityCastStart(Ability ability) { if (barType == BarType.Energy) UpdateValue(target.data); }
	public void OnTakeDamage(Modifier source, ref float damage, ref DamageType type) { if (barType == BarType.Health) UpdateValue(target.data); }
	public void OnHeal(ref float value) { if (barType == BarType.Health) UpdateValue(target.data); }
	public void OnAnimationEventEnd() => UpdateValue(target.data);

}