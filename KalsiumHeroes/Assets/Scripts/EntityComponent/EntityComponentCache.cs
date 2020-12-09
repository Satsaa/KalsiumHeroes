
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Collections;

/// <summary>
/// This class is used to cache EntityComponents for efficiency.
/// </summary>
[Serializable]
public class EntityComponentCache : EntityComponentCache<EntityComponent> { }

/// <summary>
/// This class is used to cache EntityComponents for efficiency.
/// </summary>
[Serializable]
public class EntityComponentCache<TBase> : ISerializationCallbackReceiver where TBase : EntityComponent {

	Dictionary<Type, object> cache = new Dictionary<Type, object>();

	/// <summary> Caches all loaded EntityComponents </summary>
	public void BuildCache() {
		var ecs = GameObject.FindObjectsOfType<TBase>(true);
		foreach (var ec in ecs) {
			Cache(ec);
		}
	}


	public IEnumerable<T> Enumerate<T>(bool includeInactive = false) where T : TBase {
		var type = typeof(T);
		if (includeInactive) {
			if (cache.TryGetValue(type, out var val)) {
				return val as HashSet<T>;
			} else {
				return new T[0];
			}
		} else {
			if (cache.TryGetValue(type, out var val)) {
				return (val as HashSet<T>).Where(v => v.isActiveAndEnabled);
			} else {
				return new T[0];
			}
		}
	}

	/// <summary> Adds the EntityComponent to the cache. </summary>
	public void Cache<T>(T entityComponent) where T : TBase {
		var type = entityComponent.GetType();
		while (true) {
			Type setType = typeof(HashSet<>).MakeGenericType(new[] { type });
			if (!cache.TryGetValue(type, out var set)) {
				cache[type] = set = Activator.CreateInstance(setType);
			}
			var method = setType.GetMethod("Add");
			method.Invoke(set, new object[] { entityComponent });
			if (type == typeof(TBase)) break;
			type = type.BaseType;
		}
	}

	/// <summary> Removes the EntityComponent from the cache. </summary>
	public void Uncache(TBase entityComponent) {
		var type = entityComponent.GetType();
		while (true) {
			Type setType = typeof(HashSet<>).MakeGenericType(new[] { type });
			var set = cache[type];
			var method = setType.GetMethod("Remove");
			method.Invoke(set, new object[] { entityComponent });
			if (type == typeof(TBase)) break;
			type = type.BaseType;
		}
	}


	#region Serialization

	[SerializeField, HideInInspector]
	private string[] keys;
	private EntityComponent[][] vals;

	void ISerializationCallbackReceiver.OnBeforeSerialize() {
		keys = cache.Keys.Select(v => v.AssemblyQualifiedName).ToArray();
		vals = cache.Values.Select(v => (v as IEnumerable).Cast<EntityComponent>().ToArray()).ToArray();
	}

	void ISerializationCallbackReceiver.OnAfterDeserialize() {
		if (keys != null && vals != null) {
			for (int i = 0; i < keys.Length; i++) {
				var type = Type.GetType(keys[i]);
				var ecs = vals[i];

				Type setType = typeof(HashSet<>).MakeGenericType(new[] { type });
				var set = cache[type] = Activator.CreateInstance(setType);
				var method = setType.GetMethod("Add");
				foreach (var ec in ecs) {
					method.Invoke(set, new object[] { ec });
				}
			}
		}
		keys = null;
		vals = null;
	}

	#endregion
}