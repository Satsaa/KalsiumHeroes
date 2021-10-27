
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Data;

public class UnitModifierListDisplayer : ValueHooker<Unit>, IOnUnitModifierCreate_Unit, IOnUnitModifierRemove_Unit {

	[SerializeField] GameObject prefab;
	[SerializeField] List<SerializedType<UnitModifier>> acceptedTypes;
	[SerializeField, HideInInspector] List<GameObject> statics;
	[SerializeField, HideInInspector] SerializedDictionary<UnitModifier, GameObject> dynamics;

	protected override void ReceiveValue(Unit target) {
		this.target = target;
		UpdateDynamic(target);
		Hook(target);
		if (statics.Count > 0) {
			Debug.LogWarning("You are mixing static and dynamic objects.", this);
		}
	}


	[SerializeField, HideInInspector]
	protected Unit target;

	protected IEnumerable<UnitModifier> FilterModifierDatas(IEnumerable<UnitModifier> from) {
		var seen = new HashSet<UnitModifier>();
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


	protected void UpdateStatic(Unit data) {
		var mods = FilterModifierDatas(data.baseModifiers.Cast<UnitModifier>()).ToList();
		var old = statics.ToList();
		statics.Clear();

		int i = 0;
		for (; i < mods.Count; i++) {
			if (i < old.Count) {
				statics.Add(old[i]);
				SendValue(old[i], mods[i]);
			} else {
				var go = Instantiate(prefab, transform);
				statics.Add(go);
				SendValue(go, mods[i]);
			}
		}
		for (; i < old.Count; i++) {
			Destroy(old[i]);
		}
	}


	protected void UpdateDynamic(Unit unit) {
		var mods = FilterModifiers(unit.modifiers.Get<UnitModifier>()).ToList();
		var old = dynamics.ToList();
		dynamics.Clear();

		int i = 0;
		for (; i < mods.Count; i++) {
			if (i < old.Count) {
				dynamics.Add(mods[i], old[i].Value);
				SendValue(old[i].Value, mods[i]);
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
		SendValue(go, modifier);
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