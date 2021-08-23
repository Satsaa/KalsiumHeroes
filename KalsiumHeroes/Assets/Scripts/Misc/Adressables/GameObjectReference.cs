

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Muc.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using Object = UnityEngine.Object;

/// <summary>
/// A custom (better) implementation of AssetReferenceT<T>
/// </summary>
[Serializable]
public class GameObjectReference : AssetReference<GameObject> {

	public GameObjectReference() : base() { }

	public static implicit operator GameObject(GameObjectReference v) => v.value;
	public static implicit operator bool(GameObjectReference v) => !String.IsNullOrEmpty(v.assetReference.AssetGUID);

	public Transform transform => value.transform;
	public T GetComponent<T>() => value.GetComponent<T>();
	public T[] GetComponents<T>() => value.GetComponents<T>();
	public void GetComponents<T>(List<T> results) => value.GetComponents<T>(results);
	public T GetComponentInChildren<T>() => value.GetComponentInChildren<T>();
	public T GetComponentInChildren<T>(bool includeInactive) => value.GetComponentInChildren<T>(includeInactive);
	public T[] GetComponentsInChildren<T>() => value.GetComponentsInChildren<T>();
	public T[] GetComponentsInChildren<T>(bool includeInactive) => value.GetComponentsInChildren<T>(includeInactive);
	public void GetComponentsInChildren<T>(List<T> results) => value.GetComponentsInChildren<T>(results);
	public void GetComponentsInChildren<T>(bool includeInactive, List<T> results) => value.GetComponentsInChildren<T>(includeInactive, results);
	public T GetComponentInParent<T>() => value.GetComponentInParent<T>();
	public T GetComponentInParent<T>(bool includeInactive) => value.GetComponentInParent<T>(includeInactive);
	public T[] GetComponentsInParent<T>() => value.GetComponentsInParent<T>();
	public T[] GetComponentsInParent<T>(bool includeInactive) => value.GetComponentsInParent<T>(includeInactive);
	public void GetComponentsInParent<T>(bool includeInactive, List<T> results) => value.GetComponentsInParent<T>(includeInactive, results);
	public bool TryGetComponent<T>(out T component) => value.TryGetComponent<T>(out component);

}
