using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IHas;

[CreateAssetMenu(fileName = nameof(EdgeData), menuName = "DataSources/" + nameof(EdgeData))]
public class EdgeData : MasterData, IHasDisplayName, IHasDescription {

	public override Type createTypeConstraint => typeof(Edge);

	[Tooltip("Display name of the Edge.")]
	public TextSource displayName;
	TextSource IHasDisplayName.displayName => displayName;

	[Tooltip("Description of the Edge.")]
	public TextSource description;
	TextSource IHasDescription.description => description;

}