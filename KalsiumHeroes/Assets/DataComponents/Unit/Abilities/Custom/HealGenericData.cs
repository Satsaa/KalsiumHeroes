using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(HealGenericData), menuName = "DataSources/" + nameof(HealGenericData))]
public class HealGenericData : AbilityData {
	[Header("Generic Heal Ability Data")]
	public Attribute<float> heal;

}
