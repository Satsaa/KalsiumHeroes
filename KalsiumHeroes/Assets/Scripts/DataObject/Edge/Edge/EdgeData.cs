using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(EdgeData), menuName = "DataSources/" + nameof(EdgeData))]
public class EdgeData : MasterData {

	public override Type createTypeConstraint => typeof(Edge);

}