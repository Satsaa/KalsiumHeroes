
using UnityEngine;
using Muc.Data;
using System;
using UnityEngine.Serialization;
using System.Collections.Generic;

public abstract class MasterData : DetailsObjectData {

	public override Type createTypeConstraint => typeof(Master);

	[Tooltip("Instantiated GameObject when the Master is created")]
	public GameObject container;

	[Tooltip("Automatically created modifiers for the Master.")]
	public List<ModifierData> baseModifiers;

}