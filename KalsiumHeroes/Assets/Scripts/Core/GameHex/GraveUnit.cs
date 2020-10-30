
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using HexGrid;
using UnityEngine.Serialization;

[Serializable]
public class GraveUnit {
  public UnitData unitData;
  public UnitModifierData[] modifierData;

  public GraveUnit(UnitData unitData, IEnumerable<UnitModifierData> modifierData) {
    this.unitData = ScriptableObject.Instantiate(unitData);
    this.modifierData = modifierData.Select(v => ScriptableObject.Instantiate(v)).ToArray();
  }
  public GraveUnit(Unit unit) {
    this.unitData = ScriptableObject.Instantiate(unit.data);
    this.modifierData = unit.modifiers.Select(v => ScriptableObject.Instantiate(v.data)).ToArray();
  }
}
