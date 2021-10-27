using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Muc.Editor;
using Muc.Extensions;
using Muc.Systems.RenderImages;

[DefaultExecutionOrder(-500)]
public class Unit : Master<Unit, UnitActor, IUnitHook>, IOnTurnStart_Unit, IOnDeath_Unit, IOnSpawn_Unit {

	[Tooltip("Static sprite.")]
	public AssetReference<Sprite> sprite;

	[Tooltip("The RenderObject used to render this unit in previews.")]
	public ComponentReference<RenderObject> preview;

	[Tooltip("The RenderObject used to render this unit in portraits.")]
	public ComponentReference<RenderObject> portrait;

	[Tooltip("The cost of drafting this Unit.")]
	public int draftCost = 5;

	[Tooltip("Speed determines when the unit gets to be played. The higher the speed, the higher the priority within any turn.")]
	public Speed speed;

	[Tooltip("Determines how many tiles any unit can move per turn.")]
	public Movement movement;

	[Tooltip("The health of the unit.")]
	public Health health;

	[Tooltip("Casting and extra movement costs energy.")]
	public Energy energy;

	[Tooltip("The amount of resistance to physical damage the unit posesses.")]
	public Defense defense;

	[Tooltip("The amount of resistance to magical damage the unit posesses.")]
	public Resistance resistance;

	[Tooltip("Silenced units cannot cast spells.")]
	public Silenced silenced;

	[Tooltip("Disarmed units cannot cast weapon skills.")]
	public Disarmed disarmed;

	[Tooltip("Rooted units cannot move.")]
	public Rooted rooted;

	[Serializable] public class Speed : Attribute<int> { Speed() : base(1) { } public override string identifier => "Attribute_Unit_Speed"; }
	[Serializable] public class Movement : Attribute<int> { Movement() : base(1) { } public override string identifier => "Attribute_Unit_Movement"; }
	[Serializable] public class Health : MaxAttribute<float> { Health() : base(1000, 1000) { } public override string identifier => "Attribute_Unit_Health"; }
	[Serializable] public class Energy : MaxRegenAttribute<int> { Energy() : base(10, 20, 5) { } public override string identifier => "Attribute_Unit_Energy"; }
	[Serializable] public class Defense : Attribute<int> { public override string identifier => "Attribute_Unit_Defense"; }
	[Serializable] public class Resistance : Attribute<int> { public override string identifier => "Attribute_Unit_Resistance"; }

	[Serializable]
	public abstract class Disabler : Attribute<bool> {
		public override string TooltipText(IAttribute source) => source == this && current ? DefaultTooltip(source) : null;
	}

	[Serializable] public class Silenced : Disabler { public override string identifier => "Attribute_Unit_Silenced"; }
	[Serializable] public class Disarmed : Disabler { public override string identifier => "Attribute_Unit_Disarmed"; }
	[Serializable] public class Rooted : Disabler { public override string identifier => "Attribute_Unit_Rooted"; }


	[field: SerializeField] public Tile tile { get; private set; }
	[field: SerializeField] public TileDir tileDir { get; private set; }
	[field: SerializeField] public int spawnRound { get; private set; }
	public Canvas canvas;
	public Team team;

	public bool isCurrent => Game.rounds.unit == this;


	public static T Create<T>(T source, Vector3 position, Team team) where T : Unit {
		return Create(source, v => {
			v.team = team;
			var nearTile = Game.grid.NearestTile(position.xz());
			v.SetTile(nearTile, true);
			v.SetDir(0, true);
		});
	}

	public static T Create<T>(T source, Tile tile, TileDir tileDir, Team team) where T : Unit {
		return Create(source, v => {
			v.team = team;
			v.tileDir = tileDir; // Prevent hook being called on spawn
			v.SetTile(tile, true);
			v.SetDir(tileDir, true);
		});
	}

	protected override void OnCreate() {
		base.OnCreate();
		using (var scope = new Hooks.Scope()) Game.hooks.ForEach<IOnCombatLog>(scope, v => v.OnCombatLog($"Unit spawn: {Lang.GetStr($"{identifier}_DisplayName")} ({team})"));
		using (var scope = new Hooks.Scope()) {
			this.hooks.ForEach<IOnSpawn_Unit>(scope, v => v.OnSpawn());
			tile.hooks.ForEach<IOnSpawn_Tile>(scope, v => v.OnSpawn(this));
			Game.hooks.ForEach<IOnSpawn_Game>(scope, v => v.OnSpawn(this));
		}
	}

	protected override void OnShow() {
		base.OnShow();
		actor.transform.SetPositionAndRotation(tile.center, Quaternion.Euler(actor.transform.eulerAngles.x, tileDir.ToAngle(), actor.transform.eulerAngles.z));
		transform.SetParent(actor.transform, false);
		Debug.Assert(canvas = gameObject.GetComponentInChildren<Canvas>());
	}

	public void Heal(float heal) {
		using (var scope = new Hooks.Scope()) {
			this.hooks.ForEach<IOnHeal_Unit>(scope, v => v.OnHeal(ref heal));
			tile.hooks.ForEach<IOnHeal_Tile>(scope, v => v.OnHeal(this, ref heal));
			Game.hooks.ForEach<IOnHeal_Game>(scope, v => v.OnHeal(this, ref heal));
		}
		health.current.value += Mathf.Max(0, heal);
		health.Clamp();
	}

	/// <summary> Deals damage that is calculated prior to calling this method by e.g. Ability.CalculateDamage(). </summary>
	public void DealCalculatedDamage(Modifier source, float damage, DamageType type) {
		using (var scope = new Hooks.Scope()) {
			switch (source) {
				case UnitModifier um:
					um.unit.hooks.ForEach<IOnDealDamage_Unit>(scope, v => v.OnDealDamage(um, this, damage, type));
					Game.hooks.ForEach<IOnDealDamage_Game>(scope, v => v.OnDealDamage(um, this, damage, type));
					break;
				case TileModifier tm:
					tm.tile.hooks.ForEach<IOnDealDamage_Tile>(scope, v => v.OnDealDamage(tm, this, damage, type));
					Game.hooks.ForEach<IOnDealDamage_Game>(scope, v => v.OnDealDamage(tm, this, damage, type));
					break;
				case EdgeModifier em:
					em.edge.hooks.ForEach<IOnDealDamage_Edge>(scope, v => v.OnDealDamage(em, this, damage, type));
					Game.hooks.ForEach<IOnDealDamage_Game>(scope, v => v.OnDealDamage(source, this, damage, type));
					break;
			}
		}
		using (var scope = new Hooks.Scope()) {
			this.hooks.ForEach<IOnTakeDamage_Unit>(scope, v => v.OnTakeDamage(source, ref damage, ref type));
			tile.hooks.ForEach<IOnTakeDamage_Tile>(scope, v => v.OnTakeDamage(this, source, ref damage, ref type));
			Game.hooks.ForEach<IOnTakeDamage_Game>(scope, v => v.OnTakeDamage(this, source, ref damage, ref type));
		}
		switch (type) {
			case DamageType.Physical:
				health.current.value -= (1 - defense.current / 100f) * damage;
				health.Clamp();
				break;
			case DamageType.Magical:
				health.current.value -= (1 - resistance.current / 100f) * damage;
				health.Clamp();
				break;
			case DamageType.Pure:
				health.current.value -= damage;
				health.Clamp();
				break;
			default:
				health.current.value -= damage;
				health.Clamp();
				Debug.LogWarning($"Damage type was either unknown or None. Damage was applied as {DamageType.Pure}");
				break;
		}

		if (health.current <= 0) {
			using (var scope = new Hooks.Scope()) Game.hooks.ForEach<IOnCombatLog>(scope, v => v.OnCombatLog($"Unit death: {Lang.GetStr($"{identifier}_DisplayName")} ({team})"));
			using (var scope = new Hooks.Scope()) {
				this.hooks.ForEach<IOnDeath_Unit>(scope, v => v.OnDeath());
				tile.hooks.ForEach<IOnDeath_Tile>(scope, v => v.OnDeath(this));
				Game.hooks.ForEach<IOnDeath_Game>(scope, v => v.OnDeath(this));
			}
			Remove();
		}
	}

	/// <summary> Applies any statuses caused by energy deficit or excess </summary>
	public void RefreshEnergy() {
		var deficit = 0 - energy.current;
		if (deficit > 0) {
			using (var scope = new Hooks.Scope()) {
				this.hooks.ForEach<IOnEnergyDeficit_Unit>(scope, v => v.OnEnergyDeficit(deficit));
				tile.hooks.ForEach<IOnEnergyDeficit_Tile>(scope, v => v.OnEnergyDeficit(this, deficit));
				Game.hooks.ForEach<IOnEnergyDeficit_Game>(scope, v => v.OnEnergyDeficit(this, deficit));
			}
		}
		var excess = energy.current - energy.max;
		if (excess > 0) {
			using (var scope = new Hooks.Scope()) {
				this.hooks.ForEach<IOnEnergyExcess_Unit>(scope, v => v.OnEnergyExcess(excess));
				tile.hooks.ForEach<IOnEnergyExcess_Tile>(scope, v => v.OnEnergyExcess(this, excess));
				Game.hooks.ForEach<IOnEnergyExcess_Game>(scope, v => v.OnEnergyExcess(this, excess));
			}
		}
		energy.Clamp();
	}

	public void Dispell() {
		using (var scope = new Hooks.Scope()) {
			this.hooks.ForEach<IOnDispell_Unit>(scope, v => v.OnDispell());
			tile.hooks.ForEach<IOnDispell_Tile>(scope, v => v.OnDispell(this));
			Game.hooks.ForEach<IOnDispell_Game>(scope, v => v.OnDispell(this));
		}
	}

	/// <summary> Gets the pather used by the first MoveAbility of this Unit. </summary>
	public UnitPather GetPather() {
		return UnitPathers.For(modifiers.First<MoveAbility>().rangeMode);
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
			if (reposition && actor) actor.transform.position = this.tile.center;
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
		if (reposition && shown) actor.transform.position = this.tile.center;
		using (var scope = new Hooks.Scope()) {
			this.hooks.ForEach<IOnChangePosition_Unit>(scope, v => v.OnChangePosition(orig, tile));
			tile.hooks.ForEach<IOnChangePosition_Tile>(scope, v => v.OnChangePosition(this, orig, tile));
			Game.hooks.ForEach<IOnChangePosition_Game>(scope, v => v.OnChangePosition(this, orig, tile));
		}
	}

	public void SetDir(TileDir tileDir, bool reorientate) {
		if (tileDir == this.tileDir) {
			if (reorientate && shown) actor.transform.rotation *= Quaternion.AngleAxis(tileDir.ToAngle(), Vector3.up);
			return;
		}
		if (removed) {
			this.tileDir = tileDir;
			return;
		}
		var orig = this.tileDir;
		this.tileDir = tileDir;
		if (reorientate && shown) actor.transform.rotation = Quaternion.Euler(actor.transform.eulerAngles.x, tileDir.ToAngle(), actor.transform.eulerAngles.z);
		using (var scope = new Hooks.Scope()) {
			this.hooks.ForEach<IOnChangeDirection_Unit>(scope, v => v.OnChangeDirection(orig, tileDir));
			tile.hooks.ForEach<IOnChangeDirection_Tile>(scope, v => v.OnChangeDirection(this, orig, tileDir));
			Game.hooks.ForEach<IOnChangeDirection_Game>(scope, v => v.OnChangeDirection(this, orig, tileDir));
		}
	}

	/// <summary> Gets the estimated speed of this unit after a number of rounds have passed. </summary>
	public int GetEstimatedSpeed(int roundsAhead) {
		var speed = this.speed.current.raw;
		using (var scope = new Hooks.Scope()) {
			this.hooks.ForEach<IOnGetEstimatedSpeed_Unit>(scope, v => v.OnGetEstimatedSpeed(roundsAhead, ref speed));
			tile.hooks.ForEach<IOnGetEstimatedSpeed_Tile>(scope, v => v.OnGetEstimatedSpeed(this, roundsAhead, ref speed));
			Game.hooks.ForEach<IOnGetEstimatedSpeed_Game>(scope, v => v.OnGetEstimatedSpeed(this, roundsAhead, ref speed));
		}
		return speed;
	}

	void IOnTurnStart_Unit.OnTurnStart() {
		if (Game.rounds.round <= spawnRound) return;
		energy.Regen(false);
		RefreshEnergy();
	}

	void IOnDeath_Unit.OnDeath() {
		if (shown) actor.Die();
	}

	void IOnSpawn_Unit.OnSpawn() {
		spawnRound = Mathf.Max(0, Game.rounds.round);
	}
}
