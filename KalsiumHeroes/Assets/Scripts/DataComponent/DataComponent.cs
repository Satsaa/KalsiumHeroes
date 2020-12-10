#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using Muc.Editor;
using UnityEngine;

public abstract class DataComponent : MonoBehaviour {

	[Tooltip("Source asset for data.")]
	public DataComponentData source;

	[ShowEditor]
	[Tooltip("Own data instance of source.")]
	public DataComponentData data;

	/// <summary> Actual data type required for source. </summary>
	public abstract Type dataType { get; }

	/// <summary>
	/// Adds an EntityComponent to a GameObject.
	/// </summary>
	/// <param name="target">Target GameObject</param>
	/// <param name="dataSource">Source data file which determines the type of EntityComponent created and the initialization data.</param>
	/// <returns>The created EntityComponent.</returns>
	public static DataComponent AddEntityComponent(GameObject target, DataComponentData dataSource) {
		var prevActive = target.activeInHierarchy;
		if (prevActive) target.SetActive(false);
		var res = target.AddComponent(dataSource.componentType) as DataComponent;
		res.source = dataSource;
		if (prevActive) target.SetActive(prevActive);
		return res;
	}
}

#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(DataComponent), true)]
public class EntityComponentEditor : Editor {

	SerializedProperty source;
	SerializedProperty data;

	DataComponent t => (DataComponent)target;

	void OnEnable() {
		source = serializedObject.FindProperty(nameof(DataComponent.source));
		data = serializedObject.FindProperty(nameof(DataComponent.data));
	}

	public override void OnInspectorGUI() {
		serializedObject.Update();

		using (EditorUtil.DisabledScope(Application.isPlaying))
			EditorGUILayout.ObjectField(source, t.dataType);

		using (EditorUtil.DisabledScope(!Application.isPlaying))
			EditorGUILayout.PropertyField(data);

		DrawPropertiesExcluding(serializedObject, nameof(DataComponent.source), nameof(DataComponent.data), "m_Script");

		serializedObject.ApplyModifiedProperties();
	}
}
#endif