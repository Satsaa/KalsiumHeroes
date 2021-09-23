using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(CordialInvitationStatusData), menuName = "DataSources/Units/Duelist/" + nameof(CordialInvitationStatusData))]
public class CordialInvitationStatusData : StatusData {

	public override Type createTypeConstraint => typeof(CordialInvitationStatus);

	public Attribute<int> breakRange;
	public ModifierData disgracefulBehaviour;
	public ModifierData piercingGlare;

	[HideInInspector] public Unit opponent;
	[HideInInspector] public CordialInvitationStatus opponentStatus;
	[HideInInspector] public CordialInvitationAbility duelCaster;
}
