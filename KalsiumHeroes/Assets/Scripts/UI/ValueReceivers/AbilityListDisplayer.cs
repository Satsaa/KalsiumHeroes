
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Data;

public class AbilityListDisplayer : ValueHooker<UnitData, Unit>, IOnUnitModifierCreate_Unit, IOnUnitModifierRemove_Unit {

	[SerializeField] GameObject prefab;
	[SerializeField, HideInInspector] List<GameObject> statics;
	[SerializeField, HideInInspector] SerializedDictionary<UnitModifier, GameObject> dynamics;

	protected override void ReceiveValue(UnitData data) {
		CreateStatic(data);
		if (dynamics.Any()) {
			Debug.LogWarning("You are mixing static and dynamic abilities.", this);
		}
	}

	protected override void ReceiveValue(Unit target) {
		this.target = target;
		CreateDynamics(target);
		Hook(target);
		if (statics.Any()) {
			Debug.LogWarning("You are mixing static and dynamic abilities.", this);
		}
	}


	[SerializeField, HideInInspector]
	protected Unit target;

	protected void CreateStatic(UnitData data) {
		foreach (var instantiate in statics) {
			Destroy(instantiate);
		}
		statics.Clear();
		foreach (var abilityData in data.baseModifiers.Where(v => v is AbilityData)) {
			var instantiate = Instantiate(prefab, transform);
			statics.Add(instantiate);
			ValueReceiver.SendValue(instantiate, abilityData);
		}
		foreach (var passiveData in data.baseModifiers.Where(v => v is PassiveData)) {
			var instantiate = Instantiate(prefab, transform);
			statics.Add(instantiate);
			ValueReceiver.SendValue(instantiate, passiveData);
		}
	}

	protected void CreateDynamics(Unit unit) {
		foreach (var dynamic in dynamics.Values) {
			Destroy(dynamic);
		}
		dynamics.Clear();
		foreach (var ability in unit.modifiers.Get<Ability>()) {
			CreateItem(ability);
		}
		foreach (var passive in unit.modifiers.Get<Passive>()) {
			CreateItem(passive);
		}
	}

	protected void CreateItem(UnitModifier modifier) {
		switch (modifier) {
			case Ability ability:
				var abilityGo = Instantiate(prefab, transform);
				dynamics.Add(ability, abilityGo);
				ValueReceiver.SendValue(abilityGo, ability);
				break;
			case Passive passive:
				var passiveGo = Instantiate(prefab, transform);
				dynamics.Add(passive, passiveGo);
				ValueReceiver.SendValue(passiveGo, passive);
				break;
		}
	}

	protected void RemoveItem(UnitModifier modifier) {
		foreach (var kv in dynamics) {
			if (kv.Value == modifier) {
				Destroy(kv.Value);
				dynamics.Remove(kv.Key);
				break;
			}
		}
	}

	public void OnUnitModifierCreate(UnitModifier modifier) => CreateItem(modifier);
	public void OnUnitModifierRemove(UnitModifier modifier) => RemoveItem(modifier);
}