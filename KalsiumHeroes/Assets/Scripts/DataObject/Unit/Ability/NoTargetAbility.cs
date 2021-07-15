
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NoTargetAbility : Ability {

	public new NoTargetAbilityData source => (NoTargetAbilityData)_source;
	public new NoTargetAbilityData data => (NoTargetAbilityData)_data;
	public override Type dataType => typeof(NoTargetAbilityData);

	/// <summary>
	/// Returns a list of affected Tiles if the Ability is cast.
	/// Used for highlighting and should be used when casting the ability (ensures equivalency).
	/// </summary>
	public virtual IEnumerable<Tile> GetAffectedArea() {
		return new List<Tile>() { unit.tile };
	}

	public override string CombatLog(GameEvents.Ability msg) {
		return $"{Lang.GetStr($"{unit.data.identifier}_DisplayName")} casted {Lang.GetStr($"{data.identifier}_DisplayName")}.";
	}
}