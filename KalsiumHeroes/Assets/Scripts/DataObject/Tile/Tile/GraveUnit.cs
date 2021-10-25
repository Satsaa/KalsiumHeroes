
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using HexGrid;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

[Serializable]
public class GraveUnit {

	public Unit unit;

	public GraveUnit(Unit unit) {
		this.unit = unit;
	}
}
