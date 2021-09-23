using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Status : UnitModifier, IOnTurnStart_Unit, IOnTurnEnd_Unit, IOnRoundStart, IOnDispell_Unit {

	new public StatusData source => (StatusData)_source;
	new public StatusData data => (StatusData)_data;
	public override Type dataType => typeof(StatusData);

	/// <summary> When the Unit got dispelled. </summary>
	public virtual void OnDispell() {
		if (data.dispellable.current) Remove();
	}

	public virtual void OnTurnStart() {
		switch (data.tickMode) {
			case TickMode.TurnStart:
				Tick();
				break;
			case TickMode.HybridTurn:
				Tick(false);
				break;
		}
	}

	public virtual void OnTurnEnd() {
		switch (data.tickMode) {
			case TickMode.TurnEnd:
				Tick();
				break;
			case TickMode.HybridTurn:
				if (HasExpired()) OnExpire();
				break;
		}
	}

	public virtual void OnRoundStart() {
		if (data.tickMode == TickMode.RoundStart) Tick();
	}

	/// <summary> Ticks the tick duration. </summary>
	/// <param name="doExpire">Call OnExpire if the tick duration has expired?</param>
	protected void Tick(bool doExpire = true) {
		if (!data.ticks.enabled) return;
		data.ticks.current.value++;
		OnTick();
		if (doExpire && HasExpired()) OnExpire();
	}

	/// <summary> When the tick duration is ticked. </summary>
	protected virtual void OnTick() { }

	/// <summary> When this UnitModifier expires because the tick duration was reached. </summary>
	public virtual void OnExpire() => Remove();

	/// <summary> Check if this status has expired. </summary>
	public virtual bool HasExpired() {
		return data.ticks.enabled && data.ticks.current >= data.ticks.max;
	}

	/// <summary> Check if this status would have expired after the provided round count. </summary>
	public virtual bool WouldHaveExpired(int roundsAhead) {
		if (Game.rounds.HasFinishedTurn(unit)) roundsAhead--;
		return data.ticks.enabled && data.ticks.current - roundsAhead <= 0;
	}

}