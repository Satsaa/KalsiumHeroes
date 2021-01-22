using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(CordialInvitationStatusData), menuName = "DataSources/Units/Duelist/" + nameof(CordialInvitationStatusData))]
public class CordialInvitationStatusData : StatusData {

	public override Type ownerConstraint => typeof(CordialInvitationStatus);

	public Attribute<int> breakRange;
	public ModifierData disgracefulBehaviour;
	public ModifierData piercingGlare;

	[SerializeField, HideInInspector]
	public Unit opponent;
	[SerializeField, HideInInspector]
	public CordialInvitationStatus opponentStatus;
	[SerializeField, HideInInspector]
	public CordialInvitationAbility duelCaster;
}
