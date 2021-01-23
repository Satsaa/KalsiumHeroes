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
#if UNITY_EDITOR
			if (!Application.isPlaying && PrefabUtility.IsPartOfPrefabAsset(gameObject)) {
				wasActive = gameObject.activeSelf;
				if (wasActive) gameObject.SetActive(false);
				var go = (GameObject)PrefabUtility.InstantiatePrefab(gameObject);
				Debug.Assert(!go.activeSelf);
				if (wasActive) gameObject.SetActive(true);
				return go;
			} else
#endif
			{
				wasActive = gameObject.activeSelf;
				if (wasActive) gameObject.SetActive(false);
				var go = GameObject.Instantiate(gameObject);
				Debug.Assert(!go.activeSelf);
				if (wasActive) gameObject.SetActive(true);
				return go;
			}
		}
	}

}