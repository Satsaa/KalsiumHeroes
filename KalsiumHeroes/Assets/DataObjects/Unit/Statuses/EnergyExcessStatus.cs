using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnergyExcessStatus : Status, IOnEnergyExcess_Unit, IOnAbilityCastStart_Unit, IOnAbilityCastEnd_Unit, IOnCalculateDamage_Unit {

	public new EnergyExcessStatusData data => (EnergyExcessStatusData)_data;
	public override Type dataType => typeof(EnergyExcessStatusData);

	[HideInInspector, SerializeField] Attribute<int> stacks;
	[HideInInspector, SerializeField] bool didCalculateDamage;

	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);

		data.hidden.current.ConfigureAlterer(add, this,
			applier: (v, a) => v || a > 0, // Will not override if already hidden
			updater: () => stacks.current,
			updateEvents: new[] { stacks.current.onChanged }
		);

	}


	public void OnEnergyExcess(int excess) {
		Add(excess);
	}

	public override void OnTurnEnd() {
		Clear();
	}

	void IOnAbilityCastStart_Unit.OnAbilityCastStart(Ability ability) {
		didCalculateDamage = false;
	}

	void IOnCalculateDamage_Unit.OnCalculateDamage(Modifier source, ref float damage, ref DamageType type) {
		if (source is Ability ab) {
			if (data.abiTypeMults.TryGetValue(ab.data.abilityType.current, out var mult1)) {
				damage *= mult1;
			}
			if (data.dmgTypeMults.TryGetValue(type, out var mult2)) {
				damage *= mult2;
			}
			didCalculateDamage = true;
		}
	}

	void IOnAbilityCastEnd_Unit.OnAbilityCastEnd(Ability ability) {
		if (didCalculateDamage) Clear();
	}

	private void Add(int excess) {
		stacks.current.value += excess;
		if (!data.container) return;
		var vfx = container.GetComponent<VisualEffect>();
		if (vfx) vfx.Play();
		var pts = container.GetComponent<ParticleSystem>();
		if (pts) pts.Play();
	}
	private void Clear() {
		stacks.current.value = 0;
		if (!data.container) return;
		var vfx = container.GetComponent<VisualEffect>();
		if (vfx) vfx.Stop();
		var pts = container.GetComponent<ParticleSystem>();
		if (pts) pts.Stop();
	}

	public override void OnRoundStart() {
		base.OnRoundStart();
		Debug.Log("ENERGYEXCESS STATUS ROUND: " + Game.rounds.round);
	}
}
