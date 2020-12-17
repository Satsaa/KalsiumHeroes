
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Collections;

/// <summary>
/// Stores DataComponents in collections based on their types.
/// </summary>
[Serializable]
public class DataComponentDict : DataComponentDict<DataComponent>, ISerializationCallbackReceiver { }

/// <summary>
/// Stores DataComponents in collections based on their types.
/// </summary>
[Serializable]
public class DataComponentDict<TBase> : ISerializationCallbackReceiver where TBase : DataComponent {

	Dictionary<Type, object> dict = new Dictionary<Type, object>();

	/// <summary> Caches all loaded DataComponents </summary>
	public void BuildFromScene() {
		var ecs = GameObject.FindObjectsOfType<TBase>(true);
		foreach (var ec in ecs) {
			Add(ec);
		}
	}


	public IEnumerable<TBase> Get() => Get<TBase>();

	public IEnumerable<T> Get<T>() where T : TBase {
		var type = typeof(T);
		if (dict.TryGetValue(type, out var val)) {
			return val as HashSet<T>;
		} else {
			return new T[0];
		}
	}

	/// <summary> Adds the DataComponent to the cache. </summary>
	public void Add<T>(T dataComponent) where T : TBase {
		var type = dataComponent.GetType();
		while (true) {
			Type setType = typeof(HashSet<>).MakeGenericType(new[] { type });
			if (!dict.TryGetValue(type, out var set)) {
				dict[type] = set = Activator.CreateInstance(setType);
			}
			var method = setType.GetMethod("Add");
			method.Invoke(set, new object[] { dataComponent });
			if (type == typeof(TBase)) break;
			type = type.BaseType;
		}
	}

	/// <summary> Removes the DataComponent from the cache. </summary>
	public void Remove(TBase dataComponent) {
		var type = dataComponent.GetType();
		while (true) {
			Type setType = typeof(HashSet<>).MakeGenericType(new[] { type });
			var set = dict[type];
			var method = setType.GetMethod("Remove");
			method.Invoke(set, new object[] { dataComponent });
			if (type == typeof(TBase)) break;
			type = type.BaseType;
		}
	}


	#region Serialization

	[Serializable]
	private class DataComponentArrayContainer {
		public DataComponent[] components;
		public DataComponentArrayContainer(DataComponent[] components) => this.components = components;
	}

	[SerializeField, HideInInspector]
	private string[] keys;
	[SerializeField, HideInInspector]
	private DataComponentArrayContainer[] vals;

	void ISerializationCallbackReceiver.OnBeforeSerialize() {
		keys = dict.Keys.Select(v => v.AssemblyQualifiedName).ToArray();
		vals = dict.Values.Select(v => (v as IEnumerable).Cast<DataComponent>().ToArray()).Select(v => new DataComponentArrayContainer(v)).ToArray();
	}

	void ISerializationCallbackReceiver.OnAfterDeserialize() {
		for (int i = 0; i < keys.Length; i++) {
			var type = Type.GetType(keys[i]);
			var comps = vals[i].components;

			Type setType = typeof(HashSet<>).MakeGenericType(new[] { type });
			var set = dict[type] = Activator.CreateInstance(setType);
			var method = setType.GetMethod("Add");
			foreach (var comp in comps) {
				method.Invoke(set, new object[] { comp });
			}
		}
		keys = null;
		vals = null;
	}

	#endregion
}