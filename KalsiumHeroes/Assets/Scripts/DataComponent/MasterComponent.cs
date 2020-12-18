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

	private static GameObject CreateGameObject(GameObject instantiatee) {
		if (instantiatee == null) {
			return new GameObject();
		} else {
#if UNITY_EDITOR
			if (!Application.isPlaying && PrefabUtility.IsPartOfPrefabAsset(instantiatee))
				return (GameObject)PrefabUtility.InstantiatePrefab(instantiatee);
			else
#endif
				return GameObject.Instantiate(instantiatee);
		}
	}

	public static GameObject Instantiate(MasterComponentData dataSource, Action<DataComponent> initializer = null) {
		var go = CreateGameObject(dataSource.instantiatee);
		go.AddDataComponent((DataComponentData)dataSource, initializer);
		return go;
	}
	public static GameObject Instantiate(MasterComponentData dataSource, Transform parent, Action<DataComponent> initializer = null) {
		var go = CreateGameObject(dataSource.instantiatee);
		go.transform.SetParent(parent);
		go.AddDataComponent((DataComponentData)dataSource, initializer);
		return go;
	}
	public static GameObject Instantiate(MasterComponentData dataSource, Transform parent, bool worldPositionStays, Action<DataComponent> initializer = null) {
		var go = CreateGameObject(dataSource.instantiatee);
		go.transform.SetParent(parent, worldPositionStays);
		go.AddDataComponent((DataComponentData)dataSource, initializer);
		return go;
	}
	public static GameObject Instantiate(MasterComponentData dataSource, Vector3 position, Action<DataComponent> initializer = null) {
		var go = CreateGameObject(dataSource.instantiatee);
		go.transform.position = position;
		go.AddDataComponent((DataComponentData)dataSource, initializer);
		return go;
	}
	public static GameObject Instantiate(MasterComponentData dataSource, Vector3 position, Quaternion rotation, Action<DataComponent> initializer = null) {
		var go = CreateGameObject(dataSource.instantiatee);
		go.transform.position = position;
		go.transform.rotation = rotation;
		go.AddDataComponent((DataComponentData)dataSource, initializer);
		return go;
	}
	public static GameObject Instantiate(MasterComponentData dataSource, Vector3 position, Quaternion rotation, Transform parent, Action<DataComponent> initializer = null) {
		var go = CreateGameObject(dataSource.instantiatee);
		go.transform.position = position;
		go.transform.rotation = rotation;
		go.transform.SetParent(parent);
		go.AddDataComponent((DataComponentData)dataSource, initializer);
		return go;
	}

	// With initializer and T type
	public static GameObject Instantiate<T>(MasterComponentData dataSource, Action<T> initializer = null) where T : MasterComponent {
		var go = CreateGameObject(dataSource.instantiatee);
		go.AddDataComponent<T>((DataComponentData)dataSource, initializer);
		return go;
	}
	public static GameObject Instantiate<T>(MasterComponentData dataSource, Transform parent, Action<T> initializer = null) where T : MasterComponent {
		var go = CreateGameObject(dataSource.instantiatee);
		go.transform.SetParent(parent);
		go.AddDataComponent<T>((DataComponentData)dataSource, initializer);
		return go;
	}
	public static GameObject Instantiate<T>(MasterComponentData dataSource, Transform parent, bool worldPositionStays, Action<T> initializer = null) where T : MasterComponent {
		var go = CreateGameObject(dataSource.instantiatee);
		go.transform.SetParent(parent, worldPositionStays);
		go.AddDataComponent<T>((DataComponentData)dataSource, initializer);
		return go;
	}
	public static GameObject Instantiate<T>(MasterComponentData dataSource, Vector3 position, Action<T> initializer = null) where T : MasterComponent {
		var go = CreateGameObject(dataSource.instantiatee);
		go.transform.position = position;
		go.AddDataComponent<T>((DataComponentData)dataSource, initializer);
		return go;
	}
	public static GameObject Instantiate<T>(MasterComponentData dataSource, Vector3 position, Quaternion rotation, Action<T> initializer = null) where T : MasterComponent {
		var go = CreateGameObject(dataSource.instantiatee);
		go.transform.position = position;
		go.transform.rotation = rotation;
		go.AddDataComponent<T>((DataComponentData)dataSource, initializer);
		return go;
	}
	public static GameObject Instantiate<T>(MasterComponentData dataSource, Vector3 position, Quaternion rotation, Transform parent, Action<T> initializer = null) where T : MasterComponent {
		var go = CreateGameObject(dataSource.instantiatee);
		go.transform.position = position;
		go.transform.rotation = rotation;
		go.transform.SetParent(parent);
		go.AddDataComponent<T>((DataComponentData)dataSource, initializer);
		return go;
	}
}
