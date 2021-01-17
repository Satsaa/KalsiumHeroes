#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using Muc.Editor;
using UnityEngine;
using Object = UnityEngine.Object;

public abstract class DataComponent : MonoBehaviour {

	[Tooltip("Source asset for data.")]
	public DataComponentData source;

	[ShowEditor]
	[Tooltip("Own data instance of source.")]
	public DataComponentData data;

	/// <summary> Actual data type required for source. </summary>
	public abstract Type dataType { get; }

	protected bool isBeingDestroyed;


	[Obsolete("Use the dedicated Destroy (It is immediate) function of the " + nameof(DataComponent) + ".")]
	public static void DestroyImmediate(DataComponent obj, float t = 0f) => throw new NotSupportedException();

	[Obsolete("You may want to destroy the MasterComponent instead (It is immediate) . Otherwise you can override this warning by fully qualifying DestroyImmediate: Object.DestroyImmediate")]
	public static void DestroyImmediate(GameObject obj, float t = 0f) => throw new NotSupportedException();

	[Obsolete("Use the dedicated Destroy function of the " + nameof(DataComponent) + ".")]
	public static void Destroy(DataComponent obj, float t = 0f) => throw new NotSupportedException();

	[Obsolete("You may want to destroy the MasterComponent instead. Otherwise you can override this warning by fully qualifying Destroy: Object.Destroy")]
	public static void Destroy(GameObject obj, float t = 0f) => throw new NotSupportedException();

	/// <summary> Immediately destroys this DataComponent. Duplicate destroys are handled. </summary>
	public virtual void Destroy() {
		if (isBeingDestroyed || this == null) return;
		Object.DestroyImmediate(this);
		isBeingDestroyed = true;
	}

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