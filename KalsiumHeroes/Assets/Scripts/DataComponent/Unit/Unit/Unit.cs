using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Muc.Editor;
using static UnityEngine.Mathf;
using Muc.Extensions;

public class Unit : MasterComponent<UnitModifier> {

	public UnitData unitData => (UnitData)data;
	public override Type dataType => typeof(UnitData);

	[HideInInspector]
	[Tooltip("Unit is silenced? It cannot cast spells.")]
	public SeededAttribute<bool> silenced;

	[HideInInspector]
	[Tooltip("Unit is disarmed? It cannot cast weapon skills.")]
	public SeededAttribute<bool> disarmed;

	[HideInInspector]
	[Tooltip("Unit is rooted? It cannot move.")]
	public SeededAttribute<bool> rooted;

	public Team team;
	[field: SerializeField]
	public Tile tile { get; private set; }


	protected new void Awake() {
		base.Awake();
		if (tile && (tile.unit == this || tile.unit == null)) {
			MoveTo(tile, true);
		} else {
			// Move Unit to the nearest Tile that is unoccupied
			var nearTile = Game.grid.NearestTile(transform.position.xz(), v => v.unit == null);
			if (nearTile) MoveTo(nearTile, true);
		}
		Game.dataComponents.Execute<Modifier>(v => v.OnSpawn(this));
		Game.InvokeOnAfterEvent();
		tile.modifiers.Execute<TileModifier>(v => v.OnSpawnOn(this));
		Game.InvokeOnAfterEvent();
	}

	protected new void OnDestroy() {
		if (Game.rounds.current == this) {
			Game.rounds.NextTurn();
		}
		base.OnDestroy();
	}

	void ClampHealth() {
		if (unitData.health.value < 0) {
			unitData.health.value = 0;
			return;
		}
		unitData.health.LimitValue();
	}

	public void Heal(float value) {
		unitData.health.value += modifiers.Get().Aggregate(Max(0, value), (cur, v) => Max(0, v.OnHeal(cur)));
		Game.InvokeOnAfterEvent();
		ClampHealth();
	}

	public void Damage(float value, DamageType type) {
		var total = modifiers.Get().Aggregate(Max(0, value), (cur, v) => Max(0, v.OnDamage(cur, type)));
		Game.InvokeOnAfterEvent();

		switch (type) {
			case DamageType.Physical:
				unitData.health.value -= (1 - unitData.defense.value / 100f) * total;
				ClampHealth();
				break;
			case DamageType.Magical:
				unitData.health.value -= (1 - unitData.resistance.value / 100f) * total;
				ClampHealth();
				break;
			case DamageType.Pure:
				unitData.health.value -= total;
				ClampHealth();
				break;
			case DamageType.None:
			default:
				unitData.health.value -= total;
				ClampHealth();
				Debug.LogWarning($"Damage type was either unknown or None. Damage was applied as {DamageType.Pure}");
				break;
		}

		if (unitData.health.value <= 0) {
			Game.dataComponents.Execute<Modifier>(v => v.OnDeath(this));
			Game.InvokeOnAfterEvent();
			foreach (var modifier in Game.dataComponents.Get<UnitModifier>()) modifier.OnDeath();
			Game.InvokeOnAfterEvent();
			tile.graveyard.Add(new GraveUnit(this));
			Destroy(gameObject);
		}
	}

	public void Dispell() {
		foreach (var effect in modifiers.Get<Status>()) effect.OnDispell();
		Game.InvokeOnAfterEvent();
	}

	public bool MoveTo(Tile tile, bool reposition) {
		if (tile.unit == null) {
			if (this.tile != null) this.tile.unit = null;
			tile.unit = this;
			this.tile = tile;
			if (reposition) transform.position = this.tile.center;
			return true;
		} else {
			return tile.unit == this;
		}
	}

	public int GetEstimatedSpeed(int roundsAhead) {
		return modifiers.Get().Aggregate(unitData.speed.unalteredValue, (cur, v) => cur + v.GetEstimatedSpeedGain(roundsAhead));
	}
}
