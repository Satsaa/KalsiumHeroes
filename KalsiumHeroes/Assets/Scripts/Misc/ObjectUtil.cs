#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using UnityEngine;
using Object = UnityEngine.Object;

public static class ObjectUtil {

	/// <summary> Uses Destroy or DestroyImmediate depending on the player state. </summary>
	public static void Destroy(Object obj) {
#if UNITY_EDITOR
		if (!Application.isPlaying)
			EditorApplication.delayCall += () => { if (obj && !Application.isPlaying) Object.DestroyImmediate(obj); };
		else
#endif
			Object.Destroy(obj);
	}

	/// <summary> Instates gameObject without triggering it's Awake and disabling it. </summary>
	/// <param name="wasActive">Whether the cloned object was active.</param>
	public static GameObject UnawokenGameObject(GameObject gameObject, out bool wasActive) {
		if (gameObject == null) {
			wasActive = true;
			return new GameObject();
		} else {
			wasActive = gameObject.activeSelf;
			if (wasActive) gameObject.SetActive(false);
			var go = ObjectUtil.Instantiate(gameObject);
			Debug.Assert(!go.activeSelf);
			if (wasActive) gameObject.SetActive(true);
			return go;
		}
	}

	/// <summary>
	/// Instatiates a GameObject and maintains prefab links if appropriate.
	/// </summary>
	public static GameObject Instantiate(GameObject gameObject) {
#if UNITY_EDITOR
		if (!Application.isPlaying && PrefabUtility.IsPartOfPrefabAsset(gameObject))
			return (GameObject)PrefabUtility.InstantiatePrefab(gameObject);
#endif
		return GameObject.Instantiate(gameObject);
	}

	/// <summary>
	/// Instatiates a GameObject and maintains prefab links if appropriate.
	/// </summary>
	public static GameObject Instantiate(GameObject gameObject, Transform parent) {
#if UNITY_EDITOR
		if (!Application.isPlaying && PrefabUtility.IsPartOfPrefabAsset(gameObject))
			return (GameObject)PrefabUtility.InstantiatePrefab(gameObject, parent);
#endif
		return GameObject.Instantiate(gameObject, parent);
	}

}