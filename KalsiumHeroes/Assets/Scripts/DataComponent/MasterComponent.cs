
using System;
using UnityEngine;
using Object = UnityEngine.Object;


public abstract class MasterComponent<T> : MasterComponent where T : Modifier {
	public DataComponentDict<T> modifiers = new DataComponentDict<T>();
}


public abstract class MasterComponent : DataComponent {

	public MasterComponentData masterComponentData => (MasterComponentData)data;
	public override Type dataType => typeof(MasterComponentData);

	public static GameObject Instantiate(MasterComponentData dataSource, Action<DataComponent> initializer = null) {
		var go = ObjectUtil.UnawokenGameObject(dataSource.instantiatee, out var wasActive);
		go.AddDataComponent((DataComponentData)dataSource, initializer);
		if (wasActive) go.SetActive(true);
		return go;
	}
	public static GameObject Instantiate(MasterComponentData dataSource, Transform parent, Action<DataComponent> initializer = null) {
		var go = ObjectUtil.UnawokenGameObject(dataSource.instantiatee, out var wasActive);
		go.transform.SetParent(parent);
		go.AddDataComponent((DataComponentData)dataSource, initializer);
		if (wasActive) go.SetActive(true);
		return go;
	}
	public static GameObject Instantiate(MasterComponentData dataSource, Transform parent, bool worldPositionStays, Action<DataComponent> initializer = null) {
		var go = ObjectUtil.UnawokenGameObject(dataSource.instantiatee, out var wasActive);
		go.transform.SetParent(parent, worldPositionStays);
		go.AddDataComponent((DataComponentData)dataSource, initializer);
		if (wasActive) go.SetActive(true);
		return go;
	}
	public static GameObject Instantiate(MasterComponentData dataSource, Vector3 position, Action<DataComponent> initializer = null) {
		var go = ObjectUtil.UnawokenGameObject(dataSource.instantiatee, out var wasActive);
		go.transform.position = position;
		go.AddDataComponent((DataComponentData)dataSource, initializer);
		if (wasActive) go.SetActive(true);
		return go;
	}
	public static GameObject Instantiate(MasterComponentData dataSource, Vector3 position, Quaternion rotation, Action<DataComponent> initializer = null) {
		var go = ObjectUtil.UnawokenGameObject(dataSource.instantiatee, out var wasActive);
		go.transform.position = position;
		go.transform.rotation = rotation;
		go.AddDataComponent((DataComponentData)dataSource, initializer);
		if (wasActive) go.SetActive(true);
		return go;
	}
	public static GameObject Instantiate(MasterComponentData dataSource, Vector3 position, Quaternion rotation, Transform parent, Action<DataComponent> initializer = null) {
		var go = ObjectUtil.UnawokenGameObject(dataSource.instantiatee, out var wasActive);
		go.transform.position = position;
		go.transform.rotation = rotation;
		go.transform.SetParent(parent);
		go.AddDataComponent((DataComponentData)dataSource, initializer);
		if (wasActive) go.SetActive(true);
		return go;
	}

	// With initializer and T type
	public static GameObject Instantiate<T>(MasterComponentData dataSource, Action<T> initializer = null) where T : MasterComponent {
		var go = ObjectUtil.UnawokenGameObject(dataSource.instantiatee, out var wasActive);
		go.AddDataComponent<T>((DataComponentData)dataSource, initializer);
		if (wasActive) go.SetActive(true);
		return go;
	}
	public static GameObject Instantiate<T>(MasterComponentData dataSource, Transform parent, Action<T> initializer = null) where T : MasterComponent {
		var go = ObjectUtil.UnawokenGameObject(dataSource.instantiatee, out var wasActive);
		go.transform.SetParent(parent);
		go.AddDataComponent<T>((DataComponentData)dataSource, initializer);
		if (wasActive) go.SetActive(true);
		return go;
	}
	public static GameObject Instantiate<T>(MasterComponentData dataSource, Transform parent, bool worldPositionStays, Action<T> initializer = null) where T : MasterComponent {
		var go = ObjectUtil.UnawokenGameObject(dataSource.instantiatee, out var wasActive);
		go.transform.SetParent(parent, worldPositionStays);
		go.AddDataComponent<T>((DataComponentData)dataSource, initializer);
		if (wasActive) go.SetActive(true);
		return go;
	}
	public static GameObject Instantiate<T>(MasterComponentData dataSource, Vector3 position, Action<T> initializer = null) where T : MasterComponent {
		var go = ObjectUtil.UnawokenGameObject(dataSource.instantiatee, out var wasActive);
		go.transform.position = position;
		go.AddDataComponent<T>((DataComponentData)dataSource, initializer);
		if (wasActive) go.SetActive(true);
		return go;
	}
	public static GameObject Instantiate<T>(MasterComponentData dataSource, Vector3 position, Quaternion rotation, Action<T> initializer = null) where T : MasterComponent {
		var go = ObjectUtil.UnawokenGameObject(dataSource.instantiatee, out var wasActive);
		go.transform.position = position;
		go.transform.rotation = rotation;
		go.AddDataComponent<T>((DataComponentData)dataSource, initializer);
		if (wasActive) go.SetActive(true);
		return go;
	}
	public static GameObject Instantiate<T>(MasterComponentData dataSource, Vector3 position, Quaternion rotation, Transform parent, Action<T> initializer = null) where T : MasterComponent {
		var go = ObjectUtil.UnawokenGameObject(dataSource.instantiatee, out var wasActive);
		go.transform.position = position;
		go.transform.rotation = rotation;
		go.transform.SetParent(parent);
		go.AddDataComponent<T>((DataComponentData)dataSource, initializer);
		if (wasActive) go.SetActive(true);
		return go;
	}

}