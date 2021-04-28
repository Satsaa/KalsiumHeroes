
using System;
using System.Linq;
using Muc.Editor;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;


public abstract class DataObject : ScriptableObject {

	[Tooltip("Source data instance."), SerializeField]
	protected DataObjectData _source;
	public DataObjectData source => _source;

	[Tooltip("Own data instance of source."), SerializeField, ShowEditor]
	protected DataObjectData _data;
	public DataObjectData data => _data;

	/// <summary> Actual data type required for source and data. </summary>
	public virtual Type dataType => typeof(DataObjectData);

	[field: SerializeField, HideInInspector]
	public bool removed { get; protected set; }


	protected virtual void OnCreate() { }
	protected virtual void OnRemove() { }

	/// <summary>
	/// Modifier is created or the scripts are reloaded.
	/// Also when the Modifier is removed but with add = false.
	/// Conditionally add or remove non-persistent things here.
	/// </summary>
	protected virtual void OnConfigureNonpersistent(bool add) { }


#if UNITY_EDITOR
	[UnityEditor.Callbacks.DidReloadScripts]
	private static void OnReloadScripts() {
		if (Application.isPlaying && Game.game) {
			foreach (var dobj in Game.dataObjects.Get<DataObject>().Where(v => v)) {
				dobj.OnConfigureNonpersistent(true);
			}
		}
	}
#endif

}