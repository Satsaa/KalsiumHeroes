
using UnityEngine;

[CreateAssetMenu(fileName = nameof(AbilityData), menuName = "DataSources/" + nameof(AbilityData))]
public class AbilityData : UnitModifierData {

  [Tooltip("The type of the ability. Physical = Disabled by 'Disarm', applicable damage reduced by the 'Defense' stat. Spell = Disabled by 'Silence', applicable damage reduced by 'Resistance' stat.")]
  public AbilityType type;

  [Tooltip("What types of targets can this ability target?")]
  public TargetType targetType;
  [Tooltip("The cast range of the ability.")]
  public ToggleAttribute<int> range = new ToggleAttribute<int>(1, true);
  [Tooltip("How the range is determined.")]
  public RangeMode rangeMode;
  [Tooltip("Need vision of target hex to cast?")]
  public bool requiresVision = false;

  [Tooltip("How many turns it takes for this ability to be usable again.")]
  public ToggleDualAttribute<int> cooldown = new ToggleDualAttribute<int>(false);

  [Tooltip("How many times can the ability be cast.")]
  public ToggleAttribute<int> uses = new ToggleAttribute<int>(false);

}