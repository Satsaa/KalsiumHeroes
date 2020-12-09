using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Muc.Editor;
using static UnityEngine.Mathf;

public class Unit : EntityComponent {

	public UnitData unitData => (UnitData)data;
	public override Type dataType => typeof(UnitData);

	public EntityComponentCache<Modifier> modifierCache = new EntityComponentCache<Modifier>();
	public IEnumerable<Modifier> modifiers => modifierCache.Enumerate<Modifier>(true);
	public IEnumerable<Ability> abilities => modifierCache.Enumerate<Ability>(true);
	public IEnumerable<Passive> passives => modifierCache.Enumerate<Passive>(true);
	public IEnumerable<StatusEffect> statuses => modifierCache.Enumerate<StatusEffect>(true);


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
	public GameHex hex;

	protected void OnValidate() {
		if (source && !Application.isPlaying) data = Instantiate(source);
		if (hex && hex.unit == null) {
			MovePosition(hex);
		}
	}

	protected void Awake() {
		data = Instantiate(source);
		if (hex && (hex.unit == this || hex.unit == null)) {
			MovePosition(hex);
		} else {
			// Move unit to first unoccupied hex
			foreach (var hex in Game.grid.hexes.Values) {
				if (hex.unit == null && !hex.blocked) {
					MovePosition(hex);
					break;
				}
			}
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
		unitData.health.value += modifiers.Aggregate(Max(0, value), (cur, v) => Max(0, v.OnHeal(cur)));
		ClampHealth();
	}

	public void Damage(float value, DamageType type) {
		var total = modifiers.Aggregate(Max(0, value), (cur, v) => Max(0, v.OnDamage(cur, type)));

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
			foreach (var modifier in Game.ecCache.Enumerate<Modifier>()) {
				modifier.OnDeath();
				hex.graveYard.Add(new GraveUnit(this));
				Destroy(gameObject);
			}
		}
	}

	public void Dispell() {
		foreach (var effect in modifierCache.Enumerate<StatusEffect>(true)) {
			effect.OnDispell();
		}
	}

	public bool MovePosition(GameHex hex, bool reposition = true) {
		if (hex.unit == null) {
			if (this.hex) this.hex.unit = null;
			hex.unit = this;
			this.hex = hex;
			if (reposition) transform.position = hex.center;
			return true;
		} else {
			return hex.unit == this;
		}
	}

	public void SwapPosition(GameHex hex, bool reposition = true) {
		if (hex.unit == null) {
			this.hex.unit = null;
			hex.unit = this;
			this.hex = hex;
			transform.position = hex.center;
		} else {
			var other = hex.unit;
			var sourceHex = this.hex;

			hex.unit = this;
			this.hex = hex;

			sourceHex.unit = other;
			other.hex = sourceHex;

			// The other unit is always repositioned
			if (reposition) transform.position = hex.center;
			other.transform.position = sourceHex.center;
		}
	}

}
