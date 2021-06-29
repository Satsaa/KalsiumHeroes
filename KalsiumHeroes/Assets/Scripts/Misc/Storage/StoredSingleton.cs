
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using System.IO;
using Newtonsoft.Json;
using Muc.Components.Extended;

[JsonObject(MemberSerialization.OptIn)]
public abstract class StoredSingleton : Singleton<StoredSingleton> {

	[field: SerializeField, HideInInspector]
	public string guid { get; private set; }

	protected override void Awake() {
		base.Awake();
		Load();
	}

	/// <summary> Load the storage object and populate this object </summary>
	public void Load() {
		if (instance != this) throw new InvalidOperationException("Attempting to manipulate storage within an invalid singleton.");
		Storage.LoadObject(this.GetType().Name, this);
	}

	/// <summary> Save the storage object with this object </summary>
	public void Save() {
		if (instance != this) throw new InvalidOperationException("Attempting to manipulate storage within an invalid singleton.");
		Storage.SaveObject(this.GetType().Name, this);
	}

	/// <summary> Delete the storage object for this object </summary>
	public void Delete() {
		if (instance != this) throw new InvalidOperationException("Attempting to manipulate storage within an invalid singleton.");
		Storage.DeleteObject(this.GetType().Name);
	}

}