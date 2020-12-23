using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Status : UnitModifier {

	public StatusData statusEffectData => (StatusData)data;
	public override Type dataType => typeof(StatusData);

	public override void OnTurnEnd() {
		if (statusEffectData.turnDuration.enabled && --statusEffectData.turnDuration.value <= 0) {
			OnExpire();
		}
	}

	/// <summary> When the Unit got dispelled. </summary>
	public virtual void OnDispell() {
		if (statusEffectData.dispellable) Destroy(this);
	}

	/// <summary> When this UnitModifier expires because the duration was reached. </summary>
	public virtual void OnExpire() {
		Destroy(this);
	}

	/// <summary> Based on statusEffectData.turnDuration check if this status would have expired after the provided round count. </summary>
	public bool TurnDurationWouldHaveExpired(int roundsAhead) {
		if (Game.rounds.HasFinishedTurn(unit)) roundsAhead--;
		return statusEffectData.turnDuration.enabled && statusEffectData.turnDuration.value - roundsAhead <= 0;
	}
}