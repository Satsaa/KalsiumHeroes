
using UnityEngine;
using Muc.Data;
using System;
using UnityEngine.Serialization;
using System.Collections.Generic;

public abstract class MasterData : DataObjectData {

	public override Type createTypeConstraint => typeof(Master);

	[Tooltip("Instantiated GameObject when the Master is shown")]
	public GameObject container;

	[Tooltip("Automatically created modifiers for the Master")]
	public List<ModifierData> baseModifiers;

}