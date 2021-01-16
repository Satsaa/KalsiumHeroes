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

	protected void OnValidate() {
		if (Application.isPlaying) return;
		if (source) data = Instantiate(source);
	}

	protected void Awake() {
		if (source) data = Instantiate(source);
		Game.dataComponents.Add(this);
	}

	protected void OnDestroy() {
		Game.dataComponents.Remove(this);
	}

}

#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(DataComponent), true)]
public class DataComponentEditor : Editor {

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