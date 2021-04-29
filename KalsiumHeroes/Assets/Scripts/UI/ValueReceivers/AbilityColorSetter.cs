
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.UI;
using Muc.Data;

[RequireComponent(typeof(Graphic))]
public class TeamColorSetter : ValueReceiver<Unit, Team> {

	[SerializeField] SerializedDictionary<Team, Color> colors;

	protected override void ReceiveValue(Unit target) {
		if (colors.TryGetValue(target.team, out var color)) {
			GetComponent<Graphic>().color = color;
		}
	}

	protected override void ReceiveValue(Team team) {
		if (colors.TryGetValue(team, out var color)) {
			GetComponent<Graphic>().color = color;
		}
	}

}