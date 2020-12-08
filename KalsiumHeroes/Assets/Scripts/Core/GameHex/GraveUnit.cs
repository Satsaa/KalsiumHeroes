
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using HexGrid;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

[Serializable]
public class GraveUnit {
	public UnitData unitData;
	public ModifierData[] modifierData;

	public GraveUnit(UnitData unitData, IEnumerable<ModifierData> modifierData) {
		this.unitData = Object.Instantiate(unitData);
		this.modifierData = modifierData.Select(v => Object.Instantiate(v)).ToArray();
	}
	public GraveUnit(Unit unit) {
		unitData = Object.Instantiate(unit.unitData);
		modifierData = unit.modifiers.Select(v => Object.Instantiate(v.ModifierData)).ToArray();
	}
}
