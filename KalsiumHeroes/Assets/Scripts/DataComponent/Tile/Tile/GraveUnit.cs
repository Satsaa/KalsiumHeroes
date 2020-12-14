
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
	public UnitModifierData[] unitModifierData;

	public GraveUnit(UnitData unitData, IEnumerable<UnitModifierData> modifierData) {
		this.unitData = Object.Instantiate(unitData);
		this.unitModifierData = modifierData.Select(v => Object.Instantiate(v)).ToArray();
	}
	public GraveUnit(Unit unit) {
		unitData = Object.Instantiate(unit.unitData);
		unitModifierData = unit.modifiers.Get().Select(v => Object.Instantiate(v.unitModifierData)).ToArray();
	}
}
