
using System;
using Muc.Editor;
using UnityEngine;

public static class EntityComponentGameObjectExtension {

  /// <summary>
  /// Adds an EntityComponent to a GameObject.
  /// </summary>
  /// <param name="target">Target GameObject</param>
  /// <param name="dataSource">Source data file which determines the type of EntityComponent created and the initialization data.</param>
  /// <returns>The created EntityComponent.</returns>
  public static EntityComponent AddEntityComponent(this GameObject target, EntityComponentData dataSource) {
    var prevActive = target.activeInHierarchy;
    if (prevActive) target.SetActive(false);
    var res = target.AddComponent(dataSource.componentType) as EntityComponent;
    res.source = dataSource;
    if (prevActive) target.SetActive(prevActive);
    return res;
  }

}