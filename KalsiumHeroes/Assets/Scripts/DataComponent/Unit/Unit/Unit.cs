using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Muc.Editor;
using Muc.Extensions;

[DefaultExecutionOrder(-500)]
public class Unit : MasterComponent<UnitModifier, IUnitOnEvent>, IOnTurnStart_Unit {

	public new UnitData data => (UnitData)base.data;
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

	public void Heal(float heal) {
		heal = onEvents.Aggregate<IOnHeal_Unit, float>(heal, (cur, v) => Mathf.Max(0, v.OnHeal(cur)));
		heal = tile.onEvents.Aggregate<IOnHeal_Tile, float>(heal, (cur, v) => Mathf.Max(0, v.OnHeal(this, cur)));
		heal = Game.onEvents.Aggregate<IOnHeal_Global, float>(heal, (cur, v) => Mathf.Max(0, v.OnHeal(this, cur)));
		data.health.value += Mathf.Max(0, heal);
		data.health.ClampValue();
	}

	public void DealAbilityDamage(float damage, Ability ability, DamageType damageType) {
		var calc = ability.GetCalculatedDamage(damage, damageType);
		DealCalculatedDamage(calc, damageType);
	}

	public void DealStatusDamage(float damage, Status status, DamageType damageType) {
		DealCalculatedDamage(damage, damageType);
	}

	public void DealCalculatedDamage(float damage, DamageType type) {
		var both = (damage, type);
		both = onEvents.Aggregate<IOnDamage_Unit, (float, DamageType)>(both, (cur, v) => v.OnDamage(cur.Item1, cur.Item2));
		both = tile.onEvents.Aggregate<IOnDamage_Tile, (float, DamageType)>(both, (cur, v) => v.OnDamage(this, cur.Item1, cur.Item2));
		both = Game.onEvents.Aggregate<IOnDamage_Global, (float, DamageType)>(both, (cur, v) => v.OnDamage(this, cur.Item1, cur.Item2));

		damage = both.damage;
		type = both.type;

		switch (type) {
			case DamageType.Physical:
				data.health.value -= (1 - data.defense.value / 100f) * damage;
				data.health.ClampValue();
				break;
			case DamageType.Magical:
				data.health.value -= (1 - data.resistance.value / 100f) * damage;
				data.health.ClampValue();
				break;
			case DamageType.Pure:
				data.health.value -= damage;
				data.health.ClampValue();
				break;
			default:
				data.health.value -= damage;
				data.health.ClampValue();
				Debug.LogWarning($"Damage type was either unknown or None. Damage was applied as {DamageType.Pure}");
				break;
		}

		if (data.health.value <= 0) {
			onEvents.Execute<IOnDeath_Unit>(v => v.OnDeath());
			tile.onEvents.Execute<IOnDeath_Tile>(v => v.OnDeath(this));
			Game.onEvents.Execute<IOnDeath_Global>(v => v.OnDeath(this));
			tile.graveyard.Add(new GraveUnit(this));
			Destroy(gameObject);
		}
	}

	/// <summary> Applies any statuses caused by energy deficit or excess </summary>
	public void RefreshEnergy() {
		var deficit = 0 - data.energy.value;
		if (deficit > 0) {
			onEvents.Execute<IOnEnergyDeficit_Unit>(v => v.OnEnergyDeficit(deficit));
			tile.onEvents.Execute<IOnEnergyDeficit_Tile>(v => v.OnEnergyDeficit(this, deficit));
			Game.onEvents.Execute<IOnEnergyDeficit_Global>(v => v.OnEnergyDeficit(this, deficit));
		}
		var excess = data.energy.value - data.energy.other;
		if (excess > 0) {
			onEvents.Execute<IOnEnergyExcess_Unit>(v => v.OnEnergyExcess(excess));
			tile.onEvents.Execute<IOnEnergyExcess_Tile>(v => v.OnEnergyExcess(this, excess));
			Game.onEvents.Execute<IOnEnergyExcess_Global>(v => v.OnEnergyExcess(this, excess));
		}
		data.energy.ClampValue();
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
		var value = data.speed.unalteredValue;
		value = onEvents.Aggregate<IOnGetEstimatedSpeed_Unit, int>(value, (cur, v) => cur + v.OnGetEstimatedSpeed(roundsAhead));
		value = tile.onEvents.Aggregate<IOnGetEstimatedSpeed_Tile, int>(value, (cur, v) => cur + v.OnGetEstimatedSpeed(this, roundsAhead));
		value = Game.onEvents.Aggregate<IOnGetEstimatedSpeed_Global, int>(value, (cur, v) => cur + v.OnGetEstimatedSpeed(this, roundsAhead));
		return value;
	}

	void IOnTurnStart_Unit.OnTurnStart() {
		if (Game.rounds.round <= 1) return;
		data.energy.value += data.energyRegen.value;
		RefreshEnergy();
	}
}
