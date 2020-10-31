
using System;
using Muc.Editor;
using UnityEngine;

public abstract class EntityComponent : MonoBehaviour {

  [Tooltip("Source asset for data.")]
  public EntityComponentData source;

  [ShowEditorAttribute]
  [Tooltip("Own data instance of source.")]
  public EntityComponentData data;

  /// <summary> Actual data type required for source. </summary>
  public abstract Type dataType { get; }

}