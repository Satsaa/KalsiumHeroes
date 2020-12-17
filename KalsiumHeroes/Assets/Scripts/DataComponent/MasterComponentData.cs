
using UnityEngine;
using Muc.Data;

public abstract class MasterComponentData : DataComponentData {

	[Header("Master Component Data")]
	[Tooltip("Instantiated GameObject or prefab when this MasterComponent is created")]
	public GameObject instantiatee;

}