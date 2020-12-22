
using System;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(StatusData), menuName = "DataSources/" + nameof(StatusData))]
public class StatusData : UnitModifierData {

	public override Type componentTypeConstraint => typeof(Status);

	[Header("Status Data")]
	[Tooltip("Debuff type. This status effect may, for example, be nullified if the target has resistance to the type.")]
	public DebuffType debuffType = DebuffType.None;

	[Tooltip("Is the debuff considered positive?")]
	public bool positive;

	[Tooltip("Is the debuff dispellable?")]
	public bool dispellable = true;

	[Tooltip("How long does this status effect last?")]
	public ToggleAttribute<int> turnDuration = new ToggleAttribute<int>(false);

}