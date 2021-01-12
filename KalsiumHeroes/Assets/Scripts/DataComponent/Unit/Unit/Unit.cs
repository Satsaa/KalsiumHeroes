using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Muc.Editor;
using Muc.Extensions;

public class Unit : MasterComponent<UnitModifier, IUnitOnEvent>, IOnTurnStart_Unit {

	public UnitData unitData => (UnitData)data;
	public override Type dataType => typeof(UnitData);

	[Tooltip("Silenced units cannot cast spells.")]
	public SeededAttribute<bool> silenced;

	[Tooltip("Disarmed units cannot cast weapon skills.")]
	public SeededAttribute<bool> disarmed;

	[Tooltip("Rooted units cannot move.")]
	public SeededAttribute<bool> rooted;

	public UnitActor actor;
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
		actor = GetComponentInChildren<UnitActor>();
	}

	protected void Start() {
		onEvents.Execute<IOnSpawn_Unit>(v => v.OnSpawn());
		tile.onEvents.Execute<IOnSpawn_Tile>(v => v.OnSpawn(this));
		Game.onEvents.Execute<IOnSpawn_Global>(v => v.OnSpawn(this));
	}

	protected new void OnDestroy() {
		if (Game.rounds.current == this) {
			Game.rounds.NextTurn();
		}
		base.OnDestroy();
	}

	public void Heal(float value) {
		value = onEvents.Aggregate<IOnHeal_Unit, float>(value, (cur, v) => Mathf.Max(0, v.OnHeal(cur)));
		value = tile.onEvents.Aggregate<IOnHeal_Tile, float>(value, (cur, v) => Mathf.Max(0, v.OnHeal(this, cur)));
		value = Game.onEvents.Aggregate<IOnHeal_Global, float>(value, (cur, v) => Mathf.Max(0, v.OnHeal(this, cur)));
		unitData.health.value += Mathf.Max(0, value);
		unitData.health.ClampValue();
	}

	public void Damage(float value, DamageType type) {
		var both = (value, type);
		both = onEvents.Aggregate<IOnDamage_Unit, (float, DamageType)>(both, (cur, v) => v.OnDamage(cur.Item1, cur.Item2));
		both = tile.onEvents.Aggregate<IOnDamage_Tile, (float, DamageType)>(both, (cur, v) => v.OnDamage(this, cur.Item1, cur.Item2));
		both = Game.onEvents.Aggregate<IOnDamage_Global, (float, DamageType)>(both, (cur, v) => v.OnDamage(this, cur.Item1, cur.Item2));

		value = both.value;
		type = both.type;

		switch (type) {
			case DamageType.Physical:
				unitData.health.value -= (1 - unitData.defense.value / 100f) * value;
				unitData.health.ClampValue();
				break;
			case DamageType.Magical:
				unitData.health.value -= (1 - unitData.resistance.value / 100f) * value;
				unitData.health.ClampValue();
				break;
			case DamageType.Pure:
				unitData.health.value -= value;
				unitData.health.ClampValue();
				break;
			case DamageType.None:
			default:
				unitData.health.value -= value;
				unitData.health.ClampValue();
				Debug.LogWarning($"Damage type was either unknown or None. Damage was applied as {DamageType.Pure}");
				break;
		}

		if (unitData.health.value <= 0) {
			onEvents.Execute<IOnDeath_Unit>(v => v.OnDeath());
			tile.onEvents.Execute<IOnDeath_Tile>(v => v.OnDeath(this));
			Game.onEvents.Execute<IOnDeath_Global>(v => v.OnDeath(this));
			tile.graveyard.Add(new GraveUnit(this));
			Destroy(gameObject);
		}
	}

	/// <summary> Applies any statuses caused by energy deficit or excess </summary>
	public void RefreshEnergy() {
		var deficit = 0 - unitData.energy.value;
		if (deficit > 0) Debug.Log($"Deficit energy ({deficit})");
		var excess = unitData.energy.value - unitData.energy.other;
		if (excess > 0) Debug.Log($"Excess energy ({excess})");
		unitData.energy.ClampValue();
	}

	public void Dispell() {
		onEvents.Execute<IOnDispell_Unit>(v => v.OnDispell());
		tile.onEvents.Execute<IOnDispell_Tile>(v => v.OnDispell(this));
		Game.onEvents.Execute<IOnDispell_Global>(v => v.OnDispell(this));
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
		var value = unitData.speed.unalteredValue;
		value = onEvents.Aggregate<IOnGetEstimatedSpeed_Unit, int>(value, (cur, v) => cur + v.OnGetEstimatedSpeed(roundsAhead));
		value = tile.onEvents.Aggregate<IOnGetEstimatedSpeed_Tile, int>(value, (cur, v) => cur + v.OnGetEstimatedSpeed(this, roundsAhead));
		value = Game.onEvents.Aggregate<IOnGetEstimatedSpeed_Global, int>(value, (cur, v) => cur + v.OnGetEstimatedSpeed(this, roundsAhead));
		return value;
	}

	void IOnTurnStart_Unit.OnTurnStart() {
		if (Game.rounds.round <= 1) return;
		unitData.energy.value += unitData.energyRegen.value;
		unitData.energy.ClampValue();
	}
}
