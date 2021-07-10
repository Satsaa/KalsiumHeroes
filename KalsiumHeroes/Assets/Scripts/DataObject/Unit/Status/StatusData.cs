
using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = nameof(StatusData), menuName = "DataSources/" + nameof(StatusData))]
public class StatusData : UnitModifierData {

	public override Type createTypeConstraint => typeof(Status);

	[Tooltip("Debuff type. This status effect may, for example, be nullified if the target has resistance to the type.")]
	public Attribute<DebuffType> debuffType;

	[Tooltip("Is this status displayed in the UI?")]
	public Attribute<bool> hidden;

	[Tooltip("Is this status considered positive?")]
	public Attribute<bool> positive;

	[Tooltip("Is this status dispellable?")]
	public Attribute<bool> dispellable;

	[Tooltip("How long does this status effect last?"), AttributeLabels("Current", "Limit")]
	public ToggleDualAttribute<int> ticks = new(false);

	[Tooltip("Defines the timing of ticks.")]
	public TickMode tickMode;

}
