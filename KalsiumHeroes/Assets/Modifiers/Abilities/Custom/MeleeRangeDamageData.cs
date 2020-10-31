
using UnityEngine;

[CreateAssetMenu(fileName = nameof(MeleeRangeDamageData), menuName = "DataSources/" + nameof(MeleeRangeDamageData))]
public class MeleeRangeDamageData : AbilityData {

  [Header("Melee Range Damage Ability Data")]
  public Attribute<float> damage;
  public DamageType damageType;

}
