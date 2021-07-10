
using System;
using Muc.Systems.RenderImages;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(UnitData), menuName = "DataSources/" + nameof(UnitData))]
public class UnitData : MasterData {

	public override Type createTypeConstraint => typeof(Unit);

	[Tooltip("Static sprite.")]
	[UnityEngine.Serialization.FormerlySerializedAs("staticPortrait")]
	public Sprite sprite;

	[Tooltip("The RenderObject used to render this unit in previews.")]
	public RenderObject preview;

	[Tooltip("The RenderObject used to render this unit in portraits.")]
	public RenderObject portrait;

	[Tooltip("Actor prefab for this Unit. It is instantiated as a parent.")]
	public UnitActor actor;

	[Tooltip("The cost of drafting this Unit.")]
	public int draftCost = 5;

	[Tooltip("Speed determines when the unit gets to be played. The higher the speed, the higher the priority within any turn.")]
	public Speed speed;

	[Tooltip("Determines how many tiles any unit can move per turn.")]
	public Movement movement;

	[AttributeLabels("Current", "Max")]
	[Tooltip("The health of the unit.")]
	public Health health;

	[AttributeLabels("Current", "Max")]
	[Tooltip("Casting and extra movement costs energy.")]
	public Energy energy;

	[Tooltip("How much energy is regenerated per round.")]
	public EnergyRegen energyRegen;

	[Tooltip("The amount of resistance to physical damage the unit posesses.")]
	public Defense defense;

	[Tooltip("The amount of resistance to magical damage the unit posesses.")]
	public Resistance resistance;

	[Tooltip("Silenced units cannot cast spells."), HideInInspector]
	public Silenced silenced;

	[Tooltip("Disarmed units cannot cast weapon skills."), HideInInspector]
	public Disarmed disarmed;

	[Tooltip("Rooted units cannot move."), HideInInspector]
	public Rooted rooted;

	[Serializable] public class Speed : Attribute<int> { Speed() : base(1) { } public override string identifier => "Attribute_Unit_Speed"; }
	[Serializable] public class Movement : Attribute<int> { Movement() : base(1) { } public override string identifier => "Attribute_Unit_Movement"; }
	[Serializable] public class Health : DualAttribute<float> { Health() : base(1000, 1000) { } public override string identifier => "Attribute_Unit_Health"; }
	[Serializable] public class Energy : DualAttribute<int> { Energy() : base(10, 20) { } public override string identifier => "Attribute_Unit_Energy"; }
	[Serializable] public class EnergyRegen : Attribute<int> { EnergyRegen() : base(5) { } public override string identifier => "Attribute_Unit_EnergyRegen"; }
	[Serializable] public class Defense : Attribute<int> { public override string identifier => "Attribute_Unit_Defense"; }
	[Serializable] public class Resistance : Attribute<int> { public override string identifier => "Attribute_Unit_Resistance"; }

	[Serializable]
	public abstract class Disabler : Attribute<bool> {
		public override string TooltipText(Attribute source) => source == this && value == true ? DefaultTooltip(source) : null;
	}

	[Serializable] public class Silenced : Disabler { public override string identifier => "Attribute_Unit_Silenced"; }
	[Serializable] public class Disarmed : Disabler { public override string identifier => "Attribute_Unit_Disarmed"; }
	[Serializable] public class Rooted : Disabler { public override string identifier => "Attribute_Unit_Rooted"; }

}
