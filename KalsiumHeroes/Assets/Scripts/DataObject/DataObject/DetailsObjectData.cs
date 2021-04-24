
using UnityEngine;
using Muc.Data;
using System;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = nameof(DetailsObjectData), menuName = "DataSources/" + nameof(DetailsObjectData))]
public abstract class DetailsObjectData : DataObjectData {

	[Tooltip("Display name of the DataObject.")]
	public TextSource displayName;

	[Tooltip("Description of the DataObject.")]
	public TextSource description;

}