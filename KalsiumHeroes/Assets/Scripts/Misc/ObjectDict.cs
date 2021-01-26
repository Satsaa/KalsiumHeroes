
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
			foreach (T item in val as List<T>) {
				yield return item;
			}
		}
	}

	/// <summary> Adds the Object to the cache. </summary>
	public void Add<T>(T dataObject) where T : TObj {
		var type = dataObject.GetType();
		while (true) {
			Type listType = typeof(List<>).MakeGenericType(new[] { type });
			if (!dict.TryGetValue(type, out var list)) {
				dict[type] = list = Activator.CreateInstance(listType);
			}
			var method = listType.GetMethod("Add");
			method.Invoke(list, new object[] { dataObject });
			if (type == typeof(TObj)) break;
			type = type.BaseType;
		}
	}

	/// <summary> Removes the Object from the cache. </summary>
	public void Remove(TObj dataObject) {
		var type = dataObject.GetType();
		while (true) {
			Type listType = typeof(List<>).MakeGenericType(new[] { type });
			var list = dict[type];
			var method = listType.GetMethod("Remove");
			method.Invoke(list, new object[] { dataObject });
			if (type == typeof(TObj)) break;
			type = type.BaseType;
		}
	}

	/// <summary> Returns the first Object of type T. </summary>
	public T First<T>() where T : TObj {
		var type = typeof(T);
		if (dict.TryGetValue(type, out var val)) {
			var res = (val as List<T>).FirstOrDefault();
			if (res != null) return (T)res;
		}
		throw new InvalidOperationException($"No {nameof(Modifier)} of type {typeof(T).Name}.");
	}
	/// <summary> Returns the first Object of type T or null if none exist. </summary>
	public T FirstOrDefault<T>() where T : TObj {
		var type = typeof(T);
		if (dict.TryGetValue(type, out var val)) {
			var res = (val as List<T>).FirstOrDefault();
			return (T)res;
		}
		return default;
	}

	public int IndexOf<T>(T element) where T : TObj {
		var type = typeof(T);
		if (dict.TryGetValue(type, out var val)) {
			return (val as List<T>).IndexOf(element);
		}
		return -1;
	}

	/// <summary> Returns whether an Object of type T exists. </summary>
	public bool Contains<T>() where T : TObj {
		var type = typeof(T);
		if (dict.TryGetValue(type, out var val)) {
			var list = val as List<T>;
			return list.Count > 0;
		}
		return false;
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
			Type listType = typeof(List<>).MakeGenericType(new[] { type });
			var list = dict[type] = Activator.CreateInstance(listType);
			var method = listType.GetMethod("Add");
			foreach (var comp in comps) {
				method.Invoke(list, new object[] { comp });
			}
		}
		keys = null;
		vals = null;
	}

	#endregion
}
