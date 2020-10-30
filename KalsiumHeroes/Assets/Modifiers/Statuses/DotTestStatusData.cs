
using UnityEngine;

[CreateAssetMenu(fileName = nameof(DotTestStatusData), menuName = "DataSources/" + nameof(DotTestStatusData))]
public class DotTestStatusData : StatusEffectData {

  public Attribute<float> damage;
  public DamageType damageType;

}
