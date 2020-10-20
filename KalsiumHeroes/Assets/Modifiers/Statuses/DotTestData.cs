
using UnityEngine;

[CreateAssetMenu(fileName = nameof(DotTestData), menuName = "DataSources/" + nameof(DotTestData))]
public class DotTestData : StatusEffectData {

  public Attribute<float> damage;
  public DamageType damageType;

}
