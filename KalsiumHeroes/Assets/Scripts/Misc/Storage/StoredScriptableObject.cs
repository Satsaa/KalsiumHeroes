
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using System.IO;
using Newtonsoft.Json;
using Muc.Components.Extended;

[JsonObject(MemberSerialization.OptIn)]
public abstract class StoredScriptableObject : ScriptableObject {

	public abstract string storageName { get; }

	protected virtual void Awake() {
		Load();
	}

	/// <summary> Load the storage object and populate this object </summary>
	public void Load() {
		if (Application.isPlaying || !Application.isEditor) {
			Debug.Log("Load");
			Storage.LoadObject(storageName, this);
		}
	}

	/// <summary> Save the storage object with this object </summary>
	public void Save() {
		if (Application.isPlaying || !Application.isEditor) {
			Debug.Log("Save");
			Storage.SaveObject(storageName, this);
		}
	}

	/// <summary> Delete the storage object for this object </summary>
	public void Delete() {
		Storage.DeleteObject(storageName);
	}

}
