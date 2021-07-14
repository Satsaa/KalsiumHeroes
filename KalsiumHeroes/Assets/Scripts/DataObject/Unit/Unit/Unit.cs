using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Muc.Editor;
using Muc.Extensions;

[DefaultExecutionOrder(-500)]
public class Unit : Master<UnitModifier, UnitModifierData, IUnitHook>, IOnTurnStart_Unit, IOnDeath_Unit, IOnSpawn_Unit {

	public new UnitData source => (UnitData)_source;
	public new UnitData data => (UnitData)_data;
	public override Type dataType => typeof(UnitData);

	[field: SerializeField] public Tile tile { get; private set; }
	[field: SerializeField] public TileDir tileDir { get; private set; }
	[field: SerializeField] public int spawnRound { get; private set; }
	public UnitActor actor;
	public Canvas canvas;
	public Team team;

	public bool isCurrent => Game.rounds.unit == this;


	public static Unit Create(UnitData source, Vector3 position, Team team, UnitActor actor = null) {
		var res = Create<Unit>(source, v => {
			v.team = team;
			var nearTile = Game.grid.NearestTile(v.gameObject.transform.position.xz());
			v.actor = actor ? actor : Instantiate(source.actor);
			v.SetTile(nearTile, true);
			v.SetDir(0, true);
		});
		res.gameObject.transform.SetParent(res.actor.transform, false);
		return res;
	}

	public static Unit Create(UnitData source, Tile tile, TileDir tileDir, Team team, UnitActor actor = null) {
		var res = Create<Unit>(source, v => {
			v.team = team;
			v.actor = actor ? actor : Instantiate(source.actor);
			v.tileDir = tileDir; // Prevent hook being called on spawn
			v.SetTile(tile, true);
			v.SetDir(tileDir, true);
		});
		res.gameObject.transform.SetParent(res.actor.transform, false);
		return res;
	}

	protected override void OnCreate() {
		base.OnCreate();
		Debug.Assert(canvas = gameObject.GetComponentInChildren<Canvas>());
		using (var scope = new Hooks.Scope()) {
			this.hooks.ForEach<IOnSpawn_Unit>(scope, v => v.OnSpawn());
			tile.hooks.ForEach<IOnSpawn_Tile>(scope, v => v.OnSpawn(this));
			Game.hooks.ForEach<IOnSpawn_Global>(scope, v => v.OnSpawn(this));
		}
	}

	protected override void OnRemove() {
		Debug.Log("OnRemove");
		base.OnRemove();
	}

	public void Heal(float heal) {
		using (var scope = new Hooks.Scope()) {
			this.hooks.ForEach<IOnHeal_Unit>(scope, v => v.OnHeal(ref heal));
			tile.hooks.ForEach<IOnHeal_Tile>(scope, v => v.OnHeal(this, ref heal));
			Game.hooks.ForEach<IOnHeal_Global>(scope, v => v.OnHeal(this, ref heal));
		}
		data.health.current.value += Mathf.Max(0, heal);
		data.health.Clamp();
	}

	/// <summary> Deals damage that is calculated prior to calling this method by e.g. Ability.CalculateDamage(). </summary>
	public void DealCalculatedDamage(Modifier source, float damage, DamageType type) {
		using (var scope = new Hooks.Scope()) {
			switch (source) {
				case UnitModifier um:
					um.unit.hooks.ForEach<IOnDealDamage_Unit>(scope, v => v.OnDealDamage(um, this, damage, type));
					Game.hooks.ForEach<IOnDealDamage_Global>(scope, v => v.OnDealDamage(um, this, damage, type));
					break;
				case TileModifier tm:
					tm.tile.hooks.ForEach<IOnDealDamage_Tile>(scope, v => v.OnDealDamage(tm, this, damage, type));
					Game.hooks.ForEach<IOnDealDamage_Global>(scope, v => v.OnDealDamage(tm, this, damage, type));
					break;
				case EdgeModifier em:
					em.edge.hooks.ForEach<IOnDealDamage_Edge>(scope, v => v.OnDealDamage(em, this, damage, type));
					Game.hooks.ForEach<IOnDealDamage_Global>(scope, v => v.OnDealDamage(source, this, damage, type));
					break;
			}
		}
		using (var scope = new Hooks.Scope()) {
			this.hooks.ForEach<IOnTakeDamage_Unit>(scope, v => v.OnTakeDamage(source, ref damage, ref type));
			tile.hooks.ForEach<IOnTakeDamage_Tile>(scope, v => v.OnTakeDamage(this, source, ref damage, ref type));
			Game.hooks.ForEach<IOnTakeDamage_Global>(scope, v => v.OnTakeDamage(this, source, ref damage, ref type));
		}
		switch (type) {
			case DamageType.Physical:
				data.health.current.value -= (1 - data.defense.current / 100f) * damage;
				data.health.Clamp();
				break;
			case DamageType.Magical:
				data.health.current.value -= (1 - data.resistance.current / 100f) * damage;
				data.health.Clamp();
				break;
			case DamageType.Pure:
				data.health.current.value -= damage;
				data.health.Clamp();
				break;
			default:
				data.health.current.value -= damage;
				data.health.Clamp();
				Debug.LogWarning($"Damage type was either unknown or None. Damage was applied as {DamageType.Pure}");
				break;
		}

		if (data.health.current <= 0) {
			using (var scope = new Hooks.Scope()) {
				Debug.Log("IOnDeath_Unit");
				this.hooks.ForEach<IOnDeath_Unit>(scope, v => v.OnDeath());
				tile.hooks.ForEach<IOnDeath_Tile>(scope, v => v.OnDeath(this));
				Game.hooks.ForEach<IOnDeath_Global>(scope, v => v.OnDeath(this));
			}
			tile.units.Remove(this);
			Destroy(gameObject);
			tile.graveyard.Add(new GraveUnit(this));
			Remove();
		}
	}

	/// <summary> Applies any statuses caused by energy deficit or excess </summary>
	public void RefreshEnergy() {
		var deficit = 0 - data.energy.current;
		if (deficit > 0) {
			using (var scope = new Hooks.Scope()) {
				this.hooks.ForEach<IOnEnergyDeficit_Unit>(scope, v => v.OnEnergyDeficit(deficit));
				tile.hooks.ForEach<IOnEnergyDeficit_Tile>(scope, v => v.OnEnergyDeficit(this, deficit));
				Game.hooks.ForEach<IOnEnergyDeficit_Global>(scope, v => v.OnEnergyDeficit(this, deficit));
			}
		}
		var excess = data.energy.current - data.energy.max;
		if (excess > 0) {
			using (var scope = new Hooks.Scope()) {
				this.hooks.ForEach<IOnEnergyExcess_Unit>(scope, v => v.OnEnergyExcess(excess));
				tile.hooks.ForEach<IOnEnergyExcess_Tile>(scope, v => v.OnEnergyExcess(this, excess));
				Game.hooks.ForEach<IOnEnergyExcess_Global>(scope, v => v.OnEnergyExcess(this, excess));
			}
		}
		data.energy.Clamp();
	}

	public void Dispell() {
		using (var scope = new Hooks.Scope()) {
			this.hooks.ForEach<IOnDispell_Unit>(scope, v => v.OnDispell());
			tile.hooks.ForEach<IOnDispell_Tile>(scope, v => v.OnDispell(this));
			Game.hooks.ForEach<IOnDispell_Global>(scope, v => v.OnDispell(this));
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
		if (tile == this.tile) {
			if (reposition) actor.transform.position = this.tile.center;
			return;
		}
		if (removed) {
			this.tile = tile;
			return;
		}
		var orig = this.tile;
		if (orig != null) orig.units.Remove(this);
		tile.units.Add(this);
		this.tile = tile;
		if (reposition) actor.transform.position = this.tile.center;
		using (var scope = new Hooks.Scope()) {
			this.hooks.ForEach<IOnChangePosition_Unit>(scope, v => v.OnChangePosition(orig, tile));
			tile.hooks.ForEach<IOnChangePosition_Tile>(scope, v => v.OnChangePosition(this, orig, tile));
			Game.hooks.ForEach<IOnChangePosition_Global>(scope, v => v.OnChangePosition(this, orig, tile));
		}
	}

	public void SetDir(TileDir tileDir, bool reorientate) {
		if (tileDir == this.tileDir) {
			if (reorientate) actor.transform.rotation = actor.transform.rotation * Quaternion.AngleAxis(tileDir.ToAngle(), Vector3.up);
			return;
		}
		if (removed) {
			this.tileDir = tileDir;
			return;
		}
		var orig = this.tileDir;
		this.tileDir = tileDir;
		if (reorientate) actor.transform.rotation = Quaternion.Euler(actor.transform.eulerAngles.x, tileDir.ToAngle(), actor.transform.eulerAngles.z);
		using (var scope = new Hooks.Scope()) {
			this.hooks.ForEach<IOnChangeDirection_Unit>(scope, v => v.OnChangeDirection(orig, tileDir));
			tile.hooks.ForEach<IOnChangeDirection_Tile>(scope, v => v.OnChangeDirection(this, orig, tileDir));
			Game.hooks.ForEach<IOnChangeDirection_Global>(scope, v => v.OnChangeDirection(this, orig, tileDir));
		}
	}

	/// <summary> Gets the estimated speed of this unit after a number of rounds have passed. </summary>
	public int GetEstimatedSpeed(int roundsAhead) {
		var speed = data.speed.current.raw;
		using (var scope = new Hooks.Scope()) {
			this.hooks.ForEach<IOnGetEstimatedSpeed_Unit>(scope, v => v.OnGetEstimatedSpeed(roundsAhead, ref speed));
			tile.hooks.ForEach<IOnGetEstimatedSpeed_Tile>(scope, v => v.OnGetEstimatedSpeed(this, roundsAhead, ref speed));
			Game.hooks.ForEach<IOnGetEstimatedSpeed_Global>(scope, v => v.OnGetEstimatedSpeed(this, roundsAhead, ref speed));
		}
		return speed;
	}

	void IOnTurnStart_Unit.OnTurnStart() {
		if (Game.rounds.round <= spawnRound) return;
		Debug.Log($"Turn start: {gameObject.name}");
		data.energy.Regen(false);
		RefreshEnergy();
	}

	void IOnDeath_Unit.OnDeath() {
		Debug.Log("OnDeath");
		actor.Die();
	}

	void IOnSpawn_Unit.OnSpawn() {
		spawnRound = Mathf.Max(0, Game.rounds.round);
	}
}
