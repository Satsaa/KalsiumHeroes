
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Data;

public class UnitModifierListDisplayer : ValueHooker<UnitData, Unit>, IOnUnitModifierCreate_Unit, IOnUnitModifierRemove_Unit {

	[SerializeField] GameObject prefab;
	[SerializeField] List<SerializedType<UnitModifier>> acceptedTypes;
	[SerializeField, HideInInspector] List<GameObject> statics;
	[SerializeField, HideInInspector] SerializedDictionary<UnitModifier, GameObject> dynamics;

	protected override void ReceiveValue(UnitData data) {
		UpdateStatic(data);
		if (dynamics.Any()) {
			Debug.LogWarning("You are mixing static and dynamic objects.", this);
		}
	}

	protected override void ReceiveValue(Unit target) {
		this.target = target;
		UpdateDynamic(target);
		Hook(target);
		if (statics.Any()) {
			Debug.LogWarning("You are mixing static and dynamic objects.", this);
		}
	}


	[SerializeField, HideInInspector]
	protected Unit target;

	protected IEnumerable<UnitModifierData> FilterModifierDatas(IEnumerable<UnitModifierData> from) {
		var seen = new HashSet<UnitModifierData>();
		foreach (var acceptedType in acceptedTypes) {
			foreach (var modifier in from.Where(v => acceptedType.type.IsAssignableFrom(v.GetType()))) {
				if (seen.Add(modifier)) {
					yield return modifier;
				}
			}
		}
	}

	protected IEnumerable<UnitModifier> FilterModifiers(IEnumerable<UnitModifier> from) {
		var seen = new HashSet<UnitModifier>();
		foreach (var acceptedType in acceptedTypes) {
			foreach (var modifier in from.Where(v => acceptedType.type.IsAssignableFrom(v.GetType()))) {
				if (seen.Add(modifier)) {
					yield return modifier;
				}
			}
		}
	}


	protected void UpdateStatic(UnitData data) {
		var mods = FilterModifierDatas(data.baseModifiers.Cast<UnitModifierData>()).ToList();
		var old = statics.ToList();
		statics.Clear();

		int i = 0;
		for (; i < mods.Count; i++) {
			if (i < old.Count) {
				statics.Add(old[i]);
				ValueReceiver.SendValue(old[i], mods[i]);
			} else {
				var go = Instantiate(prefab, transform);
				statics.Add(go);
				ValueReceiver.SendValue(go, mods[i]);
			}
		}
		for (; i < old.Count; i++) {
			Destroy(old[i]);
		}
	}


	protected void UpdateDynamic(Unit unit) {
		var mods = FilterModifiers(unit.modifiers.Get()).ToList();
		var old = dynamics.ToList();
		dynamics.Clear();

		int i = 0;
		for (; i < mods.Count; i++) {
			if (i < old.Count) {
				dynamics.Add(mods[i], old[i].Value);
				ValueReceiver.SendValue(old[i].Value, mods[i]);
			} else {
				CreateItem(mods[i]);
			}
		}
		for (; i < old.Count; i++) {
			Destroy(old[i].Value);
		}
	}

	protected void CreateItem(UnitModifier modifier) {
		var go = Instantiate(prefab, transform);
		dynamics.Add(modifier, go);
		ValueReceiver.SendValue(go, modifier);
	}

	protected void RemoveItem(UnitModifier modifier) {
		if (dynamics.TryGetValue(modifier, out var val)) {
			Destroy(val);
			dynamics.Remove(modifier);
		}
	}

	public void OnUnitModifierCreate(UnitModifier modifier) { if (acceptedTypes.Any(v => v.type.IsAssignableFrom(modifier.GetType()))) CreateItem(modifier); }
	public void OnUnitModifierRemove(UnitModifier modifier) { if (acceptedTypes.Any(v => v.type.IsAssignableFrom(modifier.GetType()))) RemoveItem(modifier); }
}