
using UnityEngine;

public enum AbilityType {
  None,
  [Tooltip("Special base ability. E.g. \"Move\" and \"End Turn\".")]
  Base,
  [Tooltip("Disarmed by disarm and usually deals physical damage.")]
  WeaponSkill,
  [Tooltip("Disarmed by silence and usually deals magical damage.")]
  Spell,
}
