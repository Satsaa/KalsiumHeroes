
using System;
using System.Linq;
using Muc.Editor;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;


public abstract class DetailsObject : DataObject {

	public new DetailsObjectData source => (DetailsObjectData)_source;
	public new DetailsObjectData data => (DetailsObjectData)_data;
	public override Type dataType => typeof(DetailsObjectData);

}