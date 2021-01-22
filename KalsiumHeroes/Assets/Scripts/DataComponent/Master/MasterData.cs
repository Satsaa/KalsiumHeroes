
using UnityEngine;
using Muc.Data;
using System;
using UnityEngine.Serialization;
using System.Collections.Generic;

public abstract class MasterData : DataObjectData {

	public override Type ownerConstraint => typeof(Master);

	[Header("Master Data")]

	[Tooltip("Instantiated GameObject when this MasterComponent is created"), FormerlySerializedAs("instantiatee")]
	public GameObject container;

	[Tooltip("Created modifiers for this master.")]
	public List<ModifierData> baseModifiers;


}