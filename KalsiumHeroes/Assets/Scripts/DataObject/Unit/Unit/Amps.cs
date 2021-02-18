using System;
using UnityEngine;

[Serializable]
public class Amps {
	// AbilityType amps
	public Attribute<float> GetAttribute(AbilityType abilityType) => abilityType switch {
		AbilityType.WeaponSkill => weaponSkill,
		AbilityType.Spell => spell,
		AbilityType.Skill => skill,
		_ => throw new ArgumentException(nameof(abilityType)),
	};
	[Tooltip("Outgoing damage amplifications for weapon skills. 0: no change, 1: doubled damage, -1: no damage")]
	public Attribute<float> weaponSkill;
	[Tooltip("Outgoing damage amplifications for spells. 0: no change, 1: doubled damage, -1: no damage")]
	public Attribute<float> spell;
	[Tooltip("Outgoing damage amplifications for skills. 0: no change, 1: doubled damage, -1: no damage")]
	public Attribute<float> skill;
	// DamageType amps
	public Attribute<float> GetAttribute(DamageType damageType) => damageType switch {
		DamageType.Pure => pure,
		DamageType.Physical => physical,
		DamageType.Magical => magical,
		_ => throw new ArgumentException(nameof(damageType)),
	};
	[Tooltip("Outgoing damage amplifications for physical damage. 0: no change, 1: doubled damage, -1: no damage")]
	public Attribute<float> physical;
	[Tooltip("Outgoing damage amplifications for magical damage. 0: no change, 1: doubled damage, -1: no damage")]
	public Attribute<float> magical;
	[Tooltip("Outgoing damage amplifications for pure damage. 0: no change, 1: doubled damage, -1: no damage")]
	public Attribute<float> pure;
}