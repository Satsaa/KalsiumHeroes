
using UnityEngine;

[CreateAssetMenu(fileName = nameof(StatusEffectData), menuName = "DataSources/" + nameof(StatusEffectData))]
public class StatusEffectData : UnitModifierData {

  public DebuffType debuffType = DebuffType.None;
  public bool positive;
  public bool dispellable = true;
  public ToggleAttribute<int> turnDuration = new ToggleAttribute<int>(false);

}