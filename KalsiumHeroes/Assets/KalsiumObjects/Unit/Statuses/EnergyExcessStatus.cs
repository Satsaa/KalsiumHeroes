using System;
using System.Collections;
using System.Collections.Generic;
using Muc.Data;
using UnityEngine;
using UnityEngine.VFX;

public class EnergyExcessStatus : Status, IOnEnergyExcess_Unit, IOnAbilityCastStart_Unit, IOnAbilityCastEnd_Unit, IOnCalculateDamage_Unit {

	[Tooltip("Linearly stacking outgoing damage multiplications by DamageType.")]
	public SerializedDictionary<DamageType, float> dmgTypeMults;

	[Tooltip("Linearly stacking outgoing damage multiplications by AbilityType.")]
	public SerializedDictionary<AbilityType, float> abiTypeMults;


	[HideInInspector, SerializeField] Attribute<int> stacks = new();
	[HideInInspector, SerializeField] bool didCalculateDamage;

	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);

		hidden.current.ConfigureAlterer(add, this,
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
			if (abiTypeMults.TryGetValue(ab.abilityType.current, out var mult1)) {
				damage *= mult1;
			}
			if (dmgTypeMults.TryGetValue(type, out var mult2)) {
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
		if (!container) return;
		var vfx = container.GetComponent<VisualEffect>();
		if (vfx) vfx.Play();
		var pts = container.GetComponent<ParticleSystem>();
		if (pts) pts.Play();
	}
	private void Clear() {
		stacks.current.value = 0;
		if (!container) return;
		var vfx = container.GetComponent<VisualEffect>();
		if (vfx) vfx.Stop();
		var pts = container.GetComponent<ParticleSystem>();
		if (pts) pts.Stop();
	}

}
