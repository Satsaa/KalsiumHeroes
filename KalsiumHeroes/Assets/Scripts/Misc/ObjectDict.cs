
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Collections;
using Object = UnityEngine.Object;

public interface IObjectDict {
	void Add(Object dataObject);
	void Remove(Object dataObject);
	IEnumerable<Object> Get();
}

/// <summary>
/// Stores Objects in collections based on their types.
/// </summary>
[Serializable]
public class ObjectDict<TObj> : ISerializationCallbackReceiver, IObjectDict where TObj : Object {

	Dictionary<Type, object> dict = new Dictionary<Type, object>();

	/// <summary> Enumerates Objects of the root type. </summary>
	public IEnumerable<TObj> Get() => Get<TObj>();
	/// <summary> Enumerates Objects of type T. </summary>
	public IEnumerable<T> Get<T>() where T : TObj {
		var type = typeof(T);
		if (dict.TryGetValue(type, out var val)) {
			return val as HashSet<T>;
		} else {
			return new T[0];
		}
	}

	/// <summary> Adds the Object to the cache. </summary>
	public void Add<T>(T dataObject) where T : TObj {
		var type = dataObject.GetType();
		while (true) {
			Type setType = typeof(HashSet<>).MakeGenericType(new[] { type });
			if (!dict.TryGetValue(type, out var set)) {
				dict[type] = set = Activator.CreateInstance(setType);
			}
			var method = setType.GetMethod("Add");
			method.Invoke(set, new object[] { dataObject });
			if (type == typeof(TObj)) break;
			type = type.BaseType;
		}
	}

	/// <summary> Removes the Object from the cache. </summary>
	public void Remove(TObj dataObject) {
		var type = dataObject.GetType();
		while (true) {
			Type setType = typeof(HashSet<>).MakeGenericType(new[] { type });
			var set = dict[type];
			var method = setType.GetMethod("Remove");
			method.Invoke(set, new object[] { dataObject });
			if (type == typeof(TObj)) break;
			type = type.BaseType;
		}
	}

	#region IObjectDict

	void IObjectDict.Add(Object dataObject) => Add((TObj)dataObject);
	void IObjectDict.Remove(Object dataObject) => Remove((TObj)dataObject);
	IEnumerable<Object> IObjectDict.Get() => Get<TObj>();

	#endregion


	#region Serialization

	[Serializable]
	private class ObjectArrayContainer {
		public Object[] components;
		public ObjectArrayContainer(Object[] components) => this.components = components;
	}

	[SerializeField, HideInInspector] string[] keys;
	[SerializeField, HideInInspector] ObjectArrayContainer[] vals;

	void ISerializationCallbackReceiver.OnBeforeSerialize() {
		keys = dict.Keys.Select(v => v.AssemblyQualifiedName).ToArray();
		vals = dict.Values.Select(v => (v as IEnumerable).Cast<Object>().ToArray()).Select(v => new ObjectArrayContainer(v)).ToArray();
	}

	void ISerializationCallbackReceiver.OnAfterDeserialize() {
		for (int i = 0; i < keys.Length; i++) {
			var type = Type.GetType(keys[i]);
			if (type == null) continue;
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
