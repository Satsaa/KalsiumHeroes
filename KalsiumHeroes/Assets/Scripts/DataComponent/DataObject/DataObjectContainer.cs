
using System;
using System.Linq;
using Muc.Editor;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;


[DisallowMultipleComponent]
public sealed class DataObjectContainer : MonoBehaviour {

	[ShowEditor]
	public DataObject dataObject;

}
