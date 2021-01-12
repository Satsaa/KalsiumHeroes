
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Muc.Extensions;
using System;
using UnityEngine.UI;

[RequireComponent(typeof(RoundStack))]
public class RoundStackModifier : Modifier, IOnAnimationEventEnd, IOnTurnStart_Global, IOnRoundStart {

	[HideInInspector] public RoundStack rs;

	new void Awake() {
		base.Awake();
		this.rs = GetComponent<RoundStack>();
	}

	public void OnAnimationEventEnd() => rs.Refresh();
	public void OnTurnStart(Unit unit) => rs.Refresh();
	public void OnRoundStart() => rs.Refresh();

}
