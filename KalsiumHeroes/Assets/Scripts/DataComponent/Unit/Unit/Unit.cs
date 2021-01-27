using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Muc.Editor;
using Muc.Extensions;

[DefaultExecutionOrder(-500)]
public class Unit : Master<UnitModifier, UnitModifierData, IUnitOnEvent>, IOnTurnStart_Unit {

	public new UnitData source => (UnitData)_source;
	public new UnitData data => (UnitData)_data;
	public override Type dataType => typeof(UnitData);

	[field: SerializeField]
	public Tile tile { get; private set; }
	public UnitActor actor;
	public Canvas canvas;
	public Team team;


	public static Unit Create(UnitData source, Vector3 position, Team team) {
		return Create<Unit>(source, v => {
			v.team = team;
			v.gameObject.transform.position = position;
			var nearTile = Game.grid.NearestTile(v.gameObject.transform.position.xz());
			v.SetTile(nearTile, true);
		});
	}

	public static Unit Create(UnitData source, Tile tile) {
		return Create<Unit>(source, v => {
			v.SetTile(tile, true);
		});
	}

	protected override void OnCreate() {
		base.OnCreate();
		Debug.Assert(actor = gameObject.GetComponentInChildren<UnitActor>());
		Debug.Assert(canvas = gameObject.GetComponentInChildren<Canvas>());
		using (var scope = new OnEvents.Scope()) {
			this.onEvents.ForEach<IOnSpawn_Unit>(scope, v => v.OnSpawn());
			tile.onEvents.ForEach<IOnSpawn_Tile>(scope, v => v.OnSpawn(this));
			Game.onEvents.ForEach<IOnSpawn_Global>(scope, v => v.OnSpawn(this));
		}
	}

	protected override void OnRemove() {
		if (!removed) return;
		base.OnRemove();
		if (Game.rounds.current == this) {
			Game.rounds.NextTurn();
		}
	}

	public void Heal(float heal) {
		using (var scope = new OnEvents.Scope()) {
			this.onEvents.ForEach<IOnHeal_Unit>(scope, v => v.OnHeal(ref heal));
			tile.onEvents.ForEach<IOnHeal_Tile>(scope, v => v.OnHeal(this, ref heal));
			Game.onEvents.ForEach<IOnHeal_Global>(scope, v => v.OnHeal(this, ref heal));
		}
		data.health.value += Mathf.Max(0, heal);
		data.health.ClampValue();
	}

	/// <summary> Deals damage that is calculated prior to calling this method by e.g. Ability.CalculateDamage(). </summary>
	public void DealCalculatedDamage(Modifier source, float damage, DamageType type) {
		using (var scope = new OnEvents.Scope()) {
			switch (source) {
				case UnitModifier um:
					um.unit.onEvents.ForEach<IOnDealDamage_Unit>(scope, v => v.OnDealDamage(um, this, damage, type));
					Game.onEvents.ForEach<IOnDealDamage_Global>(scope, v => v.OnDealDamage(um, this, damage, type));
					break;
				case TileModifier tm:
					tm.tile.onEvents.ForEach<IOnDealDamage_Tile>(scope, v => v.OnDealDamage(tm, this, damage, type));
					Game.onEvents.ForEach<IOnDealDamage_Global>(scope, v => v.OnDealDamage(tm, this, damage, type));
					break;
				case EdgeModifier em:
					em.edge.onEvents.ForEach<IOnDealDamage_Edge>(scope, v => v.OnDealDamage(em, this, damage, type));
					Game.onEvents.ForEach<IOnDealDamage_Global>(scope, v => v.OnDealDamage(source, this, damage, type));
					break;
			}
		}
		using (var scope = new OnEvents.Scope()) {
			this.onEvents.ForEach<IOnTakeDamage_Unit>(scope, v => v.OnTakeDamage(source, ref damage, ref type));
			tile.onEvents.ForEach<IOnTakeDamage_Tile>(scope, v => v.OnTakeDamage(this, source, ref damage, ref type));
			Game.onEvents.ForEach<IOnTakeDamage_Global>(scope, v => v.OnTakeDamage(this, source, ref damage, ref type));
		}
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
			using (var scope = new OnEvents.Scope()) {
				this.onEvents.ForEach<IOnDeath_Unit>(scope, v => v.OnDeath());
				tile.onEvents.ForEach<IOnDeath_Tile>(scope, v => v.OnDeath(this));
				Game.onEvents.ForEach<IOnDeath_Global>(scope, v => v.OnDeath(this));
			}
			tile.units.Remove(this);
			Destroy(gameObject);
			tile.graveyard.Add(new GraveUnit(this));
			Remove();
		}
	}

	/// <summary> Applies any statuses caused by energy deficit or excess </summary>
	public void RefreshEnergy() {
		var deficit = 0 - data.energy.value;
		if (deficit > 0) {
			using (var scope = new OnEvents.Scope()) {
				this.onEvents.ForEach<IOnEnergyDeficit_Unit>(scope, v => v.OnEnergyDeficit(deficit));
				tile.onEvents.ForEach<IOnEnergyDeficit_Tile>(scope, v => v.OnEnergyDeficit(this, deficit));
				Game.onEvents.ForEach<IOnEnergyDeficit_Global>(scope, v => v.OnEnergyDeficit(this, deficit));
			}
		}
		var excess = data.energy.value - data.energy.other;
		if (excess > 0) {
			using (var scope = new OnEvents.Scope()) {
				this.onEvents.ForEach<IOnEnergyExcess_Unit>(scope, v => v.OnEnergyExcess(excess));
				tile.onEvents.ForEach<IOnEnergyExcess_Tile>(scope, v => v.OnEnergyExcess(this, excess));
				Game.onEvents.ForEach<IOnEnergyExcess_Global>(scope, v => v.OnEnergyExcess(this, excess));
			}
		}
		data.energy.ClampValue();
	}

	public void Dispell() {
		using (var scope = new OnEvents.Scope()) {
			this.onEvents.ForEach<IOnDispell_Unit>(scope, v => v.OnDispell());
			tile.onEvents.ForEach<IOnDispell_Tile>(scope, v => v.OnDispell(this));
			Game.onEvents.ForEach<IOnDispell_Global>(scope, v => v.OnDispell(this));
		}
	}

	/// <summary> Gets the pather used by the first MoveAbility of this Unit. </summary>
	public UnitPather GetPather() {
		return UnitPathers.For(modifiers.First<MoveAbility>().data.rangeMode);
	}

	public bool CanMoveInDir(TileDir dir, out Tile dirTile) {
		var pather = GetPather();
		return CanMoveInDir(dir, pather, out dirTile);
	}
	public bool CanMoveInDir(TileDir dir, UnitPather pather, out Tile dirTile) {
		var from = this.tile;
		var to = dirTile = from.GetNeighbor(dir);
		if (to == null) return false;
		var edge = from.GetEdge(dir);
		return pather(this, from, edge, to);
	}

	public void SetTile(Tile tile, bool reposition) {
		if (tile == null) throw new ArgumentNullException(nameof(tile));
		if (removed) {
			this.tile = tile;
			return;
		}
		var orig = this.tile;
		if (orig != null) orig.units.Remove(this);
		tile.units.Add(this);
		this.tile = tile;
		if (reposition) gameObject.transform.position = this.tile.center;
		using (var scope = new OnEvents.Scope()) {
			this.onEvents.ForEach<IOnChangePosition_Unit>(scope, v => v.OnChangePosition(orig, tile));
			tile.onEvents.ForEach<IOnChangePosition_Tile>(scope, v => v.OnChangePosition(this, orig, tile));
			Game.onEvents.ForEach<IOnChangePosition_Global>(scope, v => v.OnChangePosition(this, orig, tile));
		}
	}

	/// <summary> Gets the estimated speed of this unit after a number of rounds have passed. </summary>
	public int GetEstimatedSpeed(int roundsAhead) {
		var speed = data.speed.unalteredValue;
		using (var scope = new OnEvents.Scope()) {
			this.onEvents.ForEach<IOnGetEstimatedSpeed_Unit>(scope, v => v.OnGetEstimatedSpeed(roundsAhead, ref speed));
			tile.onEvents.ForEach<IOnGetEstimatedSpeed_Tile>(scope, v => v.OnGetEstimatedSpeed(this, roundsAhead, ref speed));
			Game.onEvents.ForEach<IOnGetEstimatedSpeed_Global>(scope, v => v.OnGetEstimatedSpeed(this, roundsAhead, ref speed));
		}
		return speed;
	}

	void IOnTurnStart_Unit.OnTurnStart() {
		if (Game.rounds.round <= 1) return;
		Debug.Log(gameObject.name);
		data.energy.value += data.energyRegen.value;
		RefreshEnergy();
	}
}
