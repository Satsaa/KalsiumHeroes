
using System;
using Muc.Editor;
using UnityEngine;

public static class DataComponentExtensions {

	/// <summary>
	/// Adds an EntityComponent to a GameObject.
	/// </summary>
	/// <param name="target">Target GameObject</param>
	/// <param name="dataSource">Source data file which determines the type of EntityComponent created and the initialization data.</param>
	/// <returns>The created EntityComponent.</returns>
	public static DataComponent AddEntityComponent(this GameObject target, DataComponentData dataSource) {
		var wasActive = target.activeInHierarchy;
		if (wasActive) target.SetActive(false);
		var res = target.AddComponent(dataSource.componentType) as DataComponent;
		res.source = dataSource;
		if (wasActive) target.SetActive(true);
		return res;
	}

}