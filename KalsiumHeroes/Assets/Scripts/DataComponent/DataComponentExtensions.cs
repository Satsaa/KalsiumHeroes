
using System;
using Muc.Editor;
using UnityEngine;

public static class DataComponentExtensions {

	[Obsolete("Create MasterComponents with MasterComponent.Instantiate or Cast dataSource to DataComponentData to override this warning.")]
	public static DataComponent AddDataComponent(this GameObject target, MasterComponentData dataSource, Action<DataComponent> initializer = null) {
		throw new NotSupportedException("Create MasterComponents with MasterComponent.Instantiate or Cast dataSource to DataComponentData to override this warning.");
	}
	[Obsolete("Create MasterComponents with MasterComponent.Instantiate or Cast dataSource to DataComponentData to override this warning.")]
	public static DataComponent AddDataComponent<T>(this GameObject target, MasterComponentData dataSource, Action<T> initializer = null) where T : DataComponent {
		throw new NotSupportedException("Create MasterComponents with MasterComponent.Instantiate or Cast dataSource to DataComponentData to override this warning.");
	}

	/// <summary>
	/// Adds a DataComponent to a GameObject.
	/// </summary>
	/// <param name="target">Target GameObject</param>
	/// <param name="dataSource">Source data file which determines the type of DataComponent created and the initialization data.</param>
	/// <param name="initializer">Initializer ran on the created DataComponent before the GameObject is activated.</param>
	/// <typeparam name="T">Type of created DataComponent</typeparam>
	/// <returns>The created DataComponent.</returns>
	public static DataComponent AddDataComponent(this GameObject target, DataComponentData dataSource, Action<DataComponent> initializer = null) {
		var wasActive = target.activeInHierarchy;
		if (wasActive) target.SetActive(false);
		var res = target.AddComponent(dataSource.componentType) as DataComponent;
		res.source = dataSource;
		if (initializer != null) initializer(res);
		if (wasActive) target.SetActive(true);
		return res;
	}

	/// <summary>
	/// Adds a DataComponent to a GameObject.
	/// </summary>
	/// <param name="target">Target GameObject</param>
	/// <param name="dataSource">Source data file which determines the type of DataComponent created and the initialization data.</param>
	/// <param name="initializer">Initializer ran on the created DataComponent before the GameObject is activated.</param>
	/// <typeparam name="T">Type of created DataComponent</typeparam>
	/// <returns>The created DataComponent.</returns>
	public static T AddDataComponent<T>(this GameObject target, DataComponentData dataSource, Action<T> initializer = null) where T : DataComponent {
		if (!typeof(T).IsAssignableFrom(dataSource.componentType.type)) throw new InvalidCastException($"{typeof(T).Name} is not assignable to {dataSource.componentType.type.Name}");
		var wasActive = target.activeInHierarchy;
		if (wasActive) target.SetActive(false);
		var res = target.AddComponent(dataSource.componentType) as T;
		res.source = dataSource;
		if (initializer != null) initializer(res);
		if (wasActive) target.SetActive(true);
		return res;
	}

}