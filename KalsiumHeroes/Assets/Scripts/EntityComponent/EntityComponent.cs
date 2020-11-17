#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using Muc.Editor;
using UnityEngine;

public abstract class EntityComponent : MonoBehaviour {

  /// <summary>
  /// Adds an EntityComponent to a GameObject.
  /// </summary>
  /// <param name="target">Target GameObject</param>
  /// <param name="dataSource">Source data file which determines the type of EntityComponent created and the initialization data.</param>
  /// <returns>The created EntityComponent.</returns>
  public static EntityComponent AddEntityComponent(GameObject target, EntityComponentData dataSource) {
    var prevActive = target.activeInHierarchy;
    if (prevActive) target.SetActive(false);
    var res = target.AddComponent(dataSource.componentType) as EntityComponent;
    res.source = dataSource;
    if (prevActive) target.SetActive(prevActive);
    return res;
  }

  [Tooltip("Source asset for data.")]
  public EntityComponentData source;

  [ShowEditorAttribute]
  [Tooltip("Own data instance of source.")]
  public EntityComponentData data;

  /// <summary> Actual data type required for source. </summary>
  public abstract Type dataType { get; }

}

#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(EntityComponent), true)]
public class ModifierEditor : Editor {

  SerializedProperty source;
  SerializedProperty data;

  EntityComponent t => (EntityComponent)target;

  void OnEnable() {
    source = serializedObject.FindProperty(nameof(EntityComponent.source));
    data = serializedObject.FindProperty(nameof(EntityComponent.data));
  }

  public override void OnInspectorGUI() {
    serializedObject.Update();

    using (EditorUtil.DisabledScope(Application.isPlaying))
      EditorGUILayout.ObjectField(source, t.dataType);

    using (EditorUtil.DisabledScope(!Application.isPlaying))
      EditorGUILayout.PropertyField(data);

    DrawPropertiesExcluding(serializedObject, nameof(EntityComponent.source), nameof(EntityComponent.data), "m_Script");

    serializedObject.ApplyModifiedProperties();
  }
}
#endif