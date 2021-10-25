using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Status : UnitModifier, IOnTurnStart_Unit, IOnTurnEnd_Unit, IOnRoundStart, IOnDispell_Unit {

	[Tooltip("Debuff type. This status effect may, for example, be nullified if the target has resistance to the type.")]
	public Attribute<DebuffType> debuffType;

	[Tooltip("Is this status displayed in the UI?")]
	public Attribute<bool> hidden;

	[Tooltip("Is this status considered positive?")]
	public Attribute<bool> positive;

	[Tooltip("Is this status dispellable?")]
	public Attribute<bool> dispellable;

	[Tooltip("How long does this status effect last?")]
	public ToggleMaxAttribute<int> ticks = new(false);

	[Tooltip("Defines the timing of ticks.")]
	public TickMode tickMode;


	/// <summary> When the Unit got dispelled. </summary>
	public virtual void OnDispell() {
		if (dispellable.current) Remove();
	}

	public virtual void OnTurnStart() {
		switch (tickMode) {
			case TickMode.TurnStart:
				Tick();
				break;
			case TickMode.HybridTurn:
				Tick(false);
				break;
		}
	}

	public virtual void OnTurnEnd() {
		switch (tickMode) {
			case TickMode.TurnEnd:
				Tick();
				break;
			case TickMode.HybridTurn:
				if (HasExpired()) OnExpire();
				break;
		}
	}

	public virtual void OnRoundStart() {
		if (tickMode == TickMode.RoundStart) Tick();
	}

	/// <summary> Ticks the tick duration. </summary>
	/// <param name="doExpire">Call OnExpire if the tick duration has expired?</param>
	protected void Tick(bool doExpire = true) {
		if (!ticks.enabled) return;
		ticks.current.value++;
		OnTick();
		if (doExpire && HasExpired()) OnExpire();
	}

	/// <summary> When the tick duration is ticked. </summary>
	protected virtual void OnTick() { }

	/// <summary> When this UnitModifier expires because the tick duration was reached. </summary>
	public virtual void OnExpire() => Remove();

	/// <summary> Check if this status has expired. </summary>
	public virtual bool HasExpired() {
		return ticks.enabled && ticks.current >= ticks.max;
	}

	/// <summary> Check if this status would have expired after the provided round count. </summary>
	public virtual bool WouldHaveExpired(int roundsAhead) {
		if (Game.rounds.HasFinishedTurn(unit)) roundsAhead--;
		return ticks.enabled && ticks.current - roundsAhead <= 0;
	}

}