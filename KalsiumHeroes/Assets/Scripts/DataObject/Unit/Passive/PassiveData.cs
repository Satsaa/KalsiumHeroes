
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(PassiveData), menuName = "DataSources/" + nameof(PassiveData))]
public class PassiveData : UnitModifierData {

	public override Type createTypeConstraint => typeof(Passive);

}
