
using System;
using UnityEngine;
using Object = UnityEngine.Object;


public abstract class MasterComponent<T> : MasterComponent where T : Modifier {
	public DataComponentDict<T> modifiers = new DataComponentDict<T>();
}

[DisallowMultipleComponent]
public abstract class MasterComponent : DataComponent {

	public MasterComponentData masterComponentData => (MasterComponentData)data;
	public override Type dataType => typeof(MasterComponentData);

	public static GameObject Instantiate(MasterComponentData dataSource, Action<DataComponent> initializer = null) {
		var go = ObjectUtil.UnawokenGameObject(dataSource.instantiatee, out var wasActive);
		if (go.TryGetComponent(dataSource.componentType, out var comp)) ((MasterComponent)comp).source = dataSource;
		else go.AddDataComponent((DataComponentData)dataSource, initializer);
		if (wasActive) go.SetActive(true);
		return go;
	}
	public static GameObject Instantiate(MasterComponentData dataSource, Transform parent, Action<DataComponent> initializer = null) {
		var go = ObjectUtil.UnawokenGameObject(dataSource.instantiatee, out var wasActive);
		go.transform.SetParent(parent);

		if (go.TryGetComponent(dataSource.componentType, out var comp)) ((MasterComponent)comp).source = dataSource;
		else go.AddDataComponent((DataComponentData)dataSource, initializer);
		if (wasActive) go.SetActive(true);
		return go;
	}
	public static GameObject Instantiate(MasterComponentData dataSource, Transform parent, bool worldPositionStays, Action<DataComponent> initializer = null) {
		var go = ObjectUtil.UnawokenGameObject(dataSource.instantiatee, out var wasActive);
		go.transform.SetParent(parent, worldPositionStays);

		if (go.TryGetComponent(dataSource.componentType, out var comp)) ((MasterComponent)comp).source = dataSource;
		else go.AddDataComponent((DataComponentData)dataSource, initializer);
		if (wasActive) go.SetActive(true);
		return go;
	}
	public static GameObject Instantiate(MasterComponentData dataSource, Vector3 position, Action<DataComponent> initializer = null) {
		var go = ObjectUtil.UnawokenGameObject(dataSource.instantiatee, out var wasActive);
		go.transform.position = position;
		if (go.TryGetComponent(dataSource.componentType, out var comp)) ((MasterComponent)comp).source = dataSource;
		else go.AddDataComponent((DataComponentData)dataSource, initializer);
		if (wasActive) go.SetActive(true);
		return go;
	}
	public static GameObject Instantiate(MasterComponentData dataSource, Vector3 position, Quaternion rotation, Action<DataComponent> initializer = null) {
		var go = ObjectUtil.UnawokenGameObject(dataSource.instantiatee, out var wasActive);
		go.transform.position = position;
		go.transform.rotation = rotation;
		if (go.TryGetComponent(dataSource.componentType, out var comp)) ((MasterComponent)comp).source = dataSource;
		else go.AddDataComponent((DataComponentData)dataSource, initializer);
		if (wasActive) go.SetActive(true);
		return go;
	}
	public static GameObject Instantiate(MasterComponentData dataSource, Vector3 position, Quaternion rotation, Transform parent, Action<DataComponent> initializer = null) {
		var go = ObjectUtil.UnawokenGameObject(dataSource.instantiatee, out var wasActive);
		go.transform.position = position;
		go.transform.rotation = rotation;
		go.transform.SetParent(parent);
		if (go.TryGetComponent(dataSource.componentType, out var comp)) ((MasterComponent)comp).source = dataSource;
		else go.AddDataComponent((DataComponentData)dataSource, initializer);
		if (wasActive) go.SetActive(true);
		return go;
	}

}