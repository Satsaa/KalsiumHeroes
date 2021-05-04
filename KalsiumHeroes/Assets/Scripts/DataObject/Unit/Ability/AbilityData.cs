

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
	public Attribute<int> energyCost = new(0);

	[AttributeLabels("Current", "Max")]
	[Tooltip("How many turns it takes for this ability to gain a charge.")]
	public DualAttribute<int> cooldown = new(0, 0);

	[AttributeLabels("Current", "Max")]
	[Tooltip("How many charges does the ability have.")]
	public DualAttribute<int> charges = new(1, 1);

	[Tooltip("How many times can the ability be cast in total.")]
	public ToggleAttribute<int> uses = new(default, false);

	[Tooltip("Can the unit move after this spell is cast?")]
	public Attribute<bool> allowMove = new(false);

}
