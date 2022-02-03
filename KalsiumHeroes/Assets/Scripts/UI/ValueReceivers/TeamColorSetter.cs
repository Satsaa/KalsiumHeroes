
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.UI;
using Muc.Data;

[RequireComponent(typeof(Graphic))]
public class TeamColorSetter : ValueReceiver<Unit, Team> {

	protected override void ReceiveValue(Unit target) {
		GetComponent<Graphic>().color = target.team.color;
	}

	protected override void ReceiveValue(Team team) {
		GetComponent<Graphic>().color = team.color;
	}

}