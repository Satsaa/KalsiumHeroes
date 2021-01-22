using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Mathf;

[CreateAssetMenu(fileName = nameof(EdgeData), menuName = "DataSources/" + nameof(EdgeData))]
public class EdgeData : MasterData {

	public override Type ownerConstraint => typeof(Edge);

	[Tooltip("Display name of the Edge.")]
	public TextSource displayName;

	[Tooltip("Description of the Edge.")]
	public TextSource description;
}