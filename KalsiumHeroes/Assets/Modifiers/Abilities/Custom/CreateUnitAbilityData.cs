
using UnityEngine;

[CreateAssetMenu(fileName = nameof(CreateUnitAbilityData), menuName = "DataSources/" + nameof(CreateUnitAbilityData))]
public class CreateUnitAbilityData : AbilityData {

  [Header("Create Unit Ability Data")]
  public GameObject unitPrefab;

}
