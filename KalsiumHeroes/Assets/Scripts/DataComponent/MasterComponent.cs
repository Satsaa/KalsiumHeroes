#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using Muc.Editor;
using UnityEngine;
using Object = UnityEngine.Object;

public abstract class MasterComponent : DataComponent {

	/// <summary> Actual data type required for source. </summary>
	public override Type dataType => typeof(MasterComponentData);

	public static GameObject Instantiate(MasterComponentData dataSource, Action<DataComponent> initializer = null) {
		GameObject go;
		if (dataSource.instantiatee == null) {
			go = new GameObject();
		} else {
			go = GameObject.Instantiate(dataSource.instantiatee);
		}
		go.AddDataComponent((DataComponentData)dataSource, initializer);
		return go;
	}
	public static GameObject Instantiate(MasterComponentData dataSource, Transform parent, Action<DataComponent> initializer = null) {
		GameObject go;
		if (dataSource.instantiatee == null) {
			go = new GameObject();
			go.transform.SetParent(parent);
		} else {
			go = GameObject.Instantiate(dataSource.instantiatee, parent);
		}
		go.AddDataComponent((DataComponentData)dataSource, initializer);
		return go;
	}
	public static GameObject Instantiate(MasterComponentData dataSource, Transform parent, bool worldPositionStays, Action<DataComponent> initializer = null) {
		GameObject go;
		if (dataSource.instantiatee == null) {
			go = new GameObject();
			go.transform.SetParent(parent, worldPositionStays);
		} else {
			go = GameObject.Instantiate(dataSource.instantiatee, parent, worldPositionStays);
		}
		go.AddDataComponent((DataComponentData)dataSource, initializer);
		return go;
	}
	public static GameObject Instantiate(MasterComponentData dataSource, Vector3 position, Action<DataComponent> initializer = null) {
		GameObject go;
		if (dataSource.instantiatee == null) {
			go = new GameObject();
			go.transform.position = position;
		} else {
			go = GameObject.Instantiate(dataSource.instantiatee, position, dataSource.instantiatee.transform.rotation);
		}
		go.AddDataComponent((DataComponentData)dataSource, initializer);
		return go;
	}
	public static GameObject Instantiate(MasterComponentData dataSource, Vector3 position, Quaternion rotation, Action<DataComponent> initializer = null) {
		GameObject go;
		if (dataSource.instantiatee == null) {
			go = new GameObject();
			go.transform.position = position;
			go.transform.rotation = rotation;
		} else {
			go = GameObject.Instantiate(dataSource.instantiatee, position, rotation);
		}
		go.AddDataComponent((DataComponentData)dataSource, initializer);
		return go;
	}
	public static GameObject Instantiate(MasterComponentData dataSource, Vector3 position, Quaternion rotation, Transform parent, Action<DataComponent> initializer = null) {
		GameObject go;
		if (dataSource.instantiatee == null) {
			go = new GameObject();
			go.transform.position = position;
			go.transform.rotation = rotation;
			go.transform.SetParent(parent);
		} else {
			go = GameObject.Instantiate(dataSource.instantiatee, position, rotation, parent);
		}
		go.AddDataComponent((DataComponentData)dataSource, initializer);
		return go;
	}

	// With initializer and T type
	public static GameObject Instantiate<T>(MasterComponentData dataSource, Action<T> initializer = null) where T : MasterComponent {
		GameObject go;
		if (dataSource.instantiatee == null) {
			go = new GameObject();
		} else {
			go = GameObject.Instantiate(dataSource.instantiatee);
		}
		go.AddDataComponent<T>((DataComponentData)dataSource, initializer);
		return go;
	}
	public static GameObject Instantiate<T>(MasterComponentData dataSource, Transform parent, Action<T> initializer = null) where T : MasterComponent {
		GameObject go;
		if (dataSource.instantiatee == null) {
			go = new GameObject();
			go.transform.SetParent(parent);
		} else {
			go = GameObject.Instantiate(dataSource.instantiatee, parent);
		}
		go.AddDataComponent<T>((DataComponentData)dataSource, initializer);
		return go;
	}
	public static GameObject Instantiate<T>(MasterComponentData dataSource, Transform parent, bool worldPositionStays, Action<T> initializer = null) where T : MasterComponent {
		GameObject go;
		if (dataSource.instantiatee == null) {
			go = new GameObject();
			go.transform.SetParent(parent, worldPositionStays);
		} else {
			go = GameObject.Instantiate(dataSource.instantiatee, parent, worldPositionStays);
		}
		go.AddDataComponent<T>((DataComponentData)dataSource, initializer);
		return go;
	}
	public static GameObject Instantiate<T>(MasterComponentData dataSource, Vector3 position, Action<T> initializer = null) where T : MasterComponent {
		GameObject go;
		if (dataSource.instantiatee == null) {
			go = new GameObject();
			go.transform.position = position;
		} else {
			go = GameObject.Instantiate(dataSource.instantiatee, position, dataSource.instantiatee.transform.rotation);
		}
		go.AddDataComponent<T>((DataComponentData)dataSource, initializer);
		return go;
	}
	public static GameObject Instantiate<T>(MasterComponentData dataSource, Vector3 position, Quaternion rotation, Action<T> initializer = null) where T : MasterComponent {
		GameObject go;
		if (dataSource.instantiatee == null) {
			go = new GameObject();
			go.transform.position = position;
			go.transform.rotation = rotation;
		} else {
			go = GameObject.Instantiate(dataSource.instantiatee, position, rotation);
		}
		go.AddDataComponent<T>((DataComponentData)dataSource, initializer);
		return go;
	}
	public static GameObject Instantiate<T>(MasterComponentData dataSource, Vector3 position, Quaternion rotation, Transform parent, Action<T> initializer = null) where T : MasterComponent {
		GameObject go;
		if (dataSource.instantiatee == null) {
			go = new GameObject();
			go.transform.position = position;
			go.transform.rotation = rotation;
			go.transform.SetParent(parent);
		} else {
			go = GameObject.Instantiate(dataSource.instantiatee, position, rotation, parent);
		}
		go.AddDataComponent<T>((DataComponentData)dataSource, initializer);
		return go;
	}
}
