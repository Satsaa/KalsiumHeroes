
using System;
using UnityEngine;
using Object = UnityEngine.Object;


public abstract class MasterComponent<TMod, TOnEvent> : MasterComponent where TMod : Modifier where TOnEvent : IOnEvent {
	public DataComponentDict<TMod> modifiers = new DataComponentDict<TMod>();
	public OnEvents<TOnEvent> onEvents = new OnEvents<TOnEvent>();

	protected new void Awake() {
		base.Awake();
		onEvents.Add(this);
	}
}

[DisallowMultipleComponent]
public abstract class MasterComponent : DataComponent {

	public new MasterComponentData data => (MasterComponentData)base.data;
	public override Type dataType => typeof(MasterComponentData);

	/// <summary> Immediately destroys this MasterComponent and attached DataComponents. Duplicate destroys are handled. </summary>
	public override void Destroy() {
		if (this == null || isBeingDestroyed) return;
		isBeingDestroyed = true;

		// Delay until event finishes or destroy immediately
		if (OnEvents.executing) {
			isBeingDestroyedAfterOnEvent = true;
			OnEvents.onFinishEvent += Do;
		} else {
			Do();
		}

		void Do() {
			if (this == null) return; // Already destroyed??
			var go = gameObject;
			foreach (var dc in GetComponentsInChildren<DataComponent>(true)) dc.isBeingDestroyed = true;
			Object.DestroyImmediate(gameObject);
		}
	}

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