
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.UI;

public class EventStackUnitItemController : ValueHooker<Unit, int>, IOnDeath_Unit, IOnTurnStart_Unit, IOnRoundStart {

	[SerializeField, Tooltip("This will be enabled if the unit dies during it's round.")]
	GameObject enableOnDeath;

	protected override void ReceiveValue(Unit target) {
		if (this.target) {
			Debug.LogWarning("Received a duplicate value.");
			return;
		}
		this.target = target;
		if (this.round != -1 && target) Hook(target);
	}

	protected override void ReceiveValue(int round) {
		if (this.round != -1) {
			Debug.LogWarning("Received a duplicate value.");
			return;
		}
		this.round = round;
		if (this.round != -1 && target) Hook(target);
	}



	[SerializeField, HideInInspector]
	protected Unit target;

	[SerializeField, HideInInspector]
	protected int round = -1;


	public void OnDeath() {
		if (Game.rounds.round == round) {
			enableOnDeath.SetActive(true);
		}
	}

	public void OnTurnStart() {
		if (Game.rounds.round == round) {
			EventStack.eventStack.MoveReticle(target);
		}
	}

	public void OnRoundStart() {
		if (Game.rounds.round > round) {
			Unhook();
		}
	}
}