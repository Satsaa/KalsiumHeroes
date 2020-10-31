
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using HexGrid;
using UnityEngine.Serialization;

[Serializable]
public class GraveUnit {
  public UnitData unitData;
  public ModifierData[] modifierData;

  public GraveUnit(UnitData unitData, IEnumerable<ModifierData> modifierData) {
    this.unitData = ScriptableObject.Instantiate(unitData);
    this.modifierData = modifierData.Select(v => ScriptableObject.Instantiate(v)).ToArray();
  }
  public GraveUnit(Unit unit) {
    this.unitData = ScriptableObject.Instantiate(unit.unitData);
    this.modifierData = unit.modifiers.Select(v => ScriptableObject.Instantiate(v.ModifierData)).ToArray();
  }
}
