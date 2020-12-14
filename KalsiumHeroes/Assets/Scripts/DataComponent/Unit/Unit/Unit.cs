using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Muc.Editor;
using static UnityEngine.Mathf;
using Muc.Extensions;

public class Unit : DataComponent {

	public UnitData unitData => (UnitData)data;
	public override Type dataType => typeof(UnitData);

	public DataComponentDict<UnitModifier> modifiers = new DataComponentDict<UnitModifier>();


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

	protected void OnValidate() {
		if (source && !Application.isPlaying) data = Instantiate(source);
	}

	protected void Awake() {
		data = Instantiate(source);
		if (tile && (tile.unit == this || tile.unit == null)) {
			MovePosition(tile);
		} else {
			// Move Unit to the nearest Tile that is unoccupied
			var nearTile = Game.grid.NearestTile(transform.position.xz(), v => v.unit == null);
			if (nearTile) MovePosition(nearTile);
		}
	}

	protected void OnDestroy() {
		if (Game.rounds.current == this) {
			Game.rounds.NextTurn();
		}
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
		ClampHealth();
	}

	public void Damage(float value, DamageType type) {
		var total = modifiers.Get().Aggregate(Max(0, value), (cur, v) => Max(0, v.OnDamage(cur, type)));

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
			foreach (var modifier in Game.dataComponents.Get<UnitModifier>()) {
				modifier.OnDeath();
				tile.graveyard.Add(new GraveUnit(this));
				Destroy(gameObject);
			}
		}
	}

	public void Dispell() {
		foreach (var effect in modifiers.Get<StatusEffect>(true)) {
			effect.OnDispell();
		}
	}

	public bool MovePosition(Tile tile, bool reposition = true) {
		if (tile.unit == null) {
			if (this.tile) this.tile.unit = null;
			tile.unit = this;
			this.tile = tile;
			if (reposition) transform.position = tile.center;
			return true;
		} else {
			return tile.unit == this;
		}
	}

	public void SwapPosition(Tile tile, bool reposition = true) {
		if (tile.unit == null) {
			this.tile.unit = null;
			tile.unit = this;
			this.tile = tile;
			transform.position = tile.center;
		} else {
			var other = tile.unit;
			var sourceTile = this.tile;

			tile.unit = this;
			this.tile = tile;

			sourceTile.unit = other;
			other.tile = sourceTile;

			// The other unit is always repositioned
			if (reposition) transform.position = tile.center;
			other.transform.position = sourceTile.center;
		}
	}

}
