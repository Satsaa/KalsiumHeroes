
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Muc.Extensions;
using System;
using UnityEngine.UI;

[RequireComponent(typeof(RoundStack))]
public class RoundStackModifier : Modifier {

	[HideInInspector] public RoundStack rs;

	new void Awake() {
		base.Awake();
		this.rs = GetComponent<RoundStack>();
	}

	public override void OnEventEnd() => rs.Refresh();
	public override void OnTurnStart(Unit unit) => rs.Refresh();

	public override void OnRoundStart() => rs.Refresh();

}
