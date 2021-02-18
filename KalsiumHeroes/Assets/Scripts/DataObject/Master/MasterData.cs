
using UnityEngine;
using Muc.Data;
using System;
using UnityEngine.Serialization;
using System.Collections.Generic;

public abstract class MasterData : DataObjectData {

	public override Type createTypeConstraint => typeof(Master);

	[Tooltip("Instantiated GameObject when this MasterComponent is created")]
	public GameObject container;

	[Tooltip("Created modifiers for this master.")]
	public List<ModifierData> baseModifiers;


}