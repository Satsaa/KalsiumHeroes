using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Status : UnitModifier, IOnTurnEnd_Unit, IOnDispell_Unit {

	public new StatusData data => (StatusData)base.data;
	public override Type dataType => typeof(StatusData);

	public virtual void OnTurnEnd() {
		if (data.turnDuration.enabled && --data.turnDuration.value <= 0) {
			OnExpire();
		}
	}

	/// <summary> When the Unit got dispelled. </summary>
	public virtual void OnDispell() {
		if (data.dispellable.value) this.Destroy();
	}

	/// <summary> When this UnitModifier expires because the duration was reached. </summary>
	public virtual void OnExpire() {
		this.Destroy();
	}

	/// <summary> Based on statusEffectData.turnDuration check if this status would have expired after the provided round count. </summary>
	public bool TurnDurationWouldHaveExpired(int roundsAhead) {
		if (Game.rounds.HasFinishedTurn(unit)) roundsAhead--;
		return data.turnDuration.enabled && data.turnDuration.value - roundsAhead <= 0;
	}
}