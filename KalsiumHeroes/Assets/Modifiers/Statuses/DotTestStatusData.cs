
using UnityEngine;

[CreateAssetMenu(fileName = nameof(DotTestStatusData), menuName = "DataSources/" + nameof(DotTestStatusData))]
public class DotTestStatusData : StatusEffectData {

  [Header("Dot Test Data")]
  public Attribute<float> damage;
  public DamageType damageType;

}
