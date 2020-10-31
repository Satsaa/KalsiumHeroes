
using UnityEngine;

public enum AbilityType {
  None,
  [Tooltip("Special base ability. E.g. \"Move\" and \"End Turn\".")]
  Base,
  [Tooltip("Disabled by disarm and usually deals physical damage.")]
  WeaponSkill,
  [Tooltip("Disabled by silence and usually deals magical damage.")]
  Spell,
  [Tooltip("Disabled by nothing.")]
  Skill,
}
