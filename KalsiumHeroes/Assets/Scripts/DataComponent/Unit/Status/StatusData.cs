
using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = nameof(StatusData), menuName = "DataSources/" + nameof(StatusData))]
public class StatusData : UnitModifierData {

	public override Type ownerConstraint => typeof(Status);

	[Header("Status Data")]
	[Tooltip("Debuff type. This status effect may, for example, be nullified if the target has resistance to the type.")]
	public Attribute<DebuffType> debuffType = new Attribute<DebuffType>(DebuffType.None);

	[Tooltip("Is this status considered positive?")]
	public Attribute<bool> positive = new Attribute<bool>(false);

	[Tooltip("Is this status dispellable?")]
	public Attribute<bool> dispellable = new Attribute<bool>(true);

	[Tooltip("Is this status displayed in the UI?")]
	public Attribute<bool> hidden = new Attribute<bool>(false);

	[Tooltip("How long does this status effect last?"), AttributeLabels("Current", "Limit"), FormerlySerializedAs("turnDuration")]
	public ToggleDualAttribute<int> ticks = new ToggleDualAttribute<int>(0, 0, false);

	public TickMode tickMode;

}
