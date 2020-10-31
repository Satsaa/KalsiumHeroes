
using UnityEngine;

[CreateAssetMenu(fileName = nameof(AddEntityComponentAbilityData), menuName = "DataSources/" + nameof(AddEntityComponentAbilityData))]
public class AddEntityComponentAbilityData : AbilityData {

  [Header("Add Entity Component Ability Data")]

  [Tooltip("Added components. (Hint: Try adding a dot status effect!)")]
  public EntityComponentData[] components;

}
