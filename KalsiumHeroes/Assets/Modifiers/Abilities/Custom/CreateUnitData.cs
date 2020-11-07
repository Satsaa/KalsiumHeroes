
using UnityEngine;

[CreateAssetMenu(fileName = nameof(CreateUnitData), menuName = "DataSources/" + nameof(CreateUnitData))]
public class CreateUnitData : AbilityData {

  [Header("Create Unit Ability Data")]
  public GameObject unitPrefab;

}
