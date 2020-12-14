
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Collections;

/// <summary>
/// Stores EntityComponents in collections based on their types.
/// </summary>
[Serializable]
public class DataComponentDict : DataComponentDict<DataComponent> { }

/// <summary>
/// Stores EntityComponents in collections based on their types.
/// </summary>
[Serializable]
public class DataComponentDict<TBase> : ISerializationCallbackReceiver where TBase : DataComponent {

	Dictionary<Type, object> dict = new Dictionary<Type, object>();

	/// <summary> Caches all loaded EntityComponents </summary>
	public void BuildFromScene() {
		var ecs = GameObject.FindObjectsOfType<TBase>(true);
		foreach (var ec in ecs) {
			Add(ec);
		}
	}


	public IEnumerable<TBase> Get(bool includeInactive = false) => Get<TBase>();

	public IEnumerable<T> Get<T>(bool includeInactive = false) where T : TBase {
		var type = typeof(T);
		if (includeInactive) {
			if (dict.TryGetValue(type, out var val)) {
				return val as HashSet<T>;
			} else {
				return new T[0];
			}
		} else {
			if (dict.TryGetValue(type, out var val)) {
				return (val as HashSet<T>).Where(v => v.isActiveAndEnabled);
			} else {
				return new T[0];
			}
		}
	}

	/// <summary> Adds the EntityComponent to the cache. </summary>
	public void Add<T>(T entityComponent) where T : TBase {
		var type = entityComponent.GetType();
		while (true) {
			Type setType = typeof(HashSet<>).MakeGenericType(new[] { type });
			if (!dict.TryGetValue(type, out var set)) {
				dict[type] = set = Activator.CreateInstance(setType);
			}
			var method = setType.GetMethod("Add");
			method.Invoke(set, new object[] { entityComponent });
			if (type == typeof(TBase)) break;
			type = type.BaseType;
		}
	}

	/// <summary> Removes the EntityComponent from the cache. </summary>
	public void Remove(TBase entityComponent) {
		var type = entityComponent.GetType();
		while (true) {
			Type setType = typeof(HashSet<>).MakeGenericType(new[] { type });
			var set = dict[type];
			var method = setType.GetMethod("Remove");
			method.Invoke(set, new object[] { entityComponent });
			if (type == typeof(TBase)) break;
			type = type.BaseType;
		}
	}


	#region Serialization

	[SerializeField, HideInInspector]
	private string[] keys;
	private DataComponent[][] vals;

	void ISerializationCallbackReceiver.OnBeforeSerialize() {
		keys = dict.Keys.Select(v => v.AssemblyQualifiedName).ToArray();
		vals = dict.Values.Select(v => (v as IEnumerable).Cast<DataComponent>().ToArray()).ToArray();
	}

	void ISerializationCallbackReceiver.OnAfterDeserialize() {
		if (keys != null && vals != null) {
			for (int i = 0; i < keys.Length; i++) {
				var type = Type.GetType(keys[i]);
				var ecs = vals[i];

				Type setType = typeof(HashSet<>).MakeGenericType(new[] { type });
				var set = dict[type] = Activator.CreateInstance(setType);
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