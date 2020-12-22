using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(DisarmStatusData), menuName = "DataSources/" + nameof(DisarmStatusData))]
public class DisarmStatusData : StatusData {

	public override Type componentConstraint => typeof(DisarmStatus);

}
