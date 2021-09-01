
using UnityEngine;
using Muc.Data;
using System;
using UnityEngine.Serialization;
using System.Collections.Generic;

public abstract class MasterData : DataObjectData {

	public override Type createTypeConstraint => typeof(Master);
	public virtual Type actorTypeConstraint => typeof(Actor);

	[Tooltip("Automatically created modifiers for the Master")]
	public List<ModifierData> baseModifiers;

	[Tooltip("Instantiated GameObject when the Master is shown. Actors are more defined containers.")]
	public ComponentReference<Actor> actor;

}
