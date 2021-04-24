
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class AbilityListDisplayer : ValueReceiver<UnitData, Unit> {

	[SerializeField] GameObject prefab;
	[SerializeField, HideInInspector] List<GameObject> instantiates;

	protected override void ReceiveValue(UnitData data) {
		foreach (var instantiate in instantiates) {
			Destroy(instantiate);
		}
		instantiates.Clear();
		foreach (var modifier in data.baseModifiers.Where(v => v is AbilityData || v is PassiveData)) {
			var instantiate = Instantiate(prefab, transform);
			instantiates.Add(instantiate);
			ValueReceiver.SendValue(instantiate, modifier);
		}
	}

	protected override void ReceiveValue(Unit unit) {
		ReceiveValue(unit.data);
		//TODO
	}

}