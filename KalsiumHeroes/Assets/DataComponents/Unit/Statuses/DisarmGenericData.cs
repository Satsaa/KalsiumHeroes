using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(DisarmGenericData), menuName = "DataSources/" + nameof(DisarmGenericData))]
public class DisarmGenericData : StatusData {

	public override Type componentTypeConstraint => typeof(DisarmGeneric);

}
