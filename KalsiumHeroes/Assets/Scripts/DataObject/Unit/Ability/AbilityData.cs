

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(AbilityData), menuName = "DataSources/" + nameof(AbilityData))]
public abstract class AbilityData : UnitModifierData {

	public override Type createTypeConstraint => typeof(Ability);

	[Tooltip("Type of the ability.")]
	public AbilityType abilityType;

	[Tooltip("The amount of energy required to cast this ability.")]
	public EnergyCost energyCost;

	[Tooltip("How many turns it takes for this ability to gain a charge.")]
	public Cooldown cooldown;

	[Tooltip("How many charges does the ability have.")]
	public Charges charges;

	[Tooltip("How many times can the ability be cast in total.")]
	public Uses uses;

	[Tooltip("Can the unit move after this spell is cast?")]
	public AllowMove allowMove;

	[Serializable]
	public class AbilityType : Attribute<global::AbilityType> {
		public override string identifier => "Attribute_Ability_AbilityType";
		public override string TooltipText(IAttribute source) => current == global::AbilityType.Base ? null : DefaultTooltip(source, Lang.GetStr("Targets"));
		public override string Format(bool isSource) => Lang.GetStr($"{identifier}_{current.value.ToString()}");
	}

	[Serializable]
	public class EnergyCost : Attribute<int> {
		public override string identifier => "Attribute_Ability_EnergyCost";
		public override string TooltipText(IAttribute source) {
			if (current != 0) return DefaultTooltip(source);
			return null;
		}
	}

	[Serializable]
	public class Cooldown : MaxAttribute<int> {
		Cooldown() : base(0, 1) { }
		public override string identifier => "Attribute_Ability_Cooldown";
		public override string TooltipText(IAttribute source) {
			if (max != 1) return DefaultTooltip(source);
			return null;
		}
	}

	[Serializable]
	public class Charges : MaxAttribute<int> {
		Charges() : base(1, 1) { }
		public override string identifier => "Attribute_Ability_Charges";
		public override string TooltipText(IAttribute source) {
			if (max != 1) return DefaultTooltip(source);
			return null;
		}
	}

	[Serializable]
	public class Uses : ToggleAttribute<int> {
		Uses() : base(false) { }
		public override string identifier => "Attribute_Ability_Uses";
		public override string TooltipText(IAttribute source) {
			if (enabled) return DefaultTooltip(source);
			return null;
		}
	}

	[Serializable]
	public class AllowMove : Attribute<bool> {
		public override string identifier => "Attribute_Ability_AllowMove";
		public override string TooltipText(IAttribute source) {
			if (current == true) return DefaultTooltip(source);
			return null;
		}
	}
}
