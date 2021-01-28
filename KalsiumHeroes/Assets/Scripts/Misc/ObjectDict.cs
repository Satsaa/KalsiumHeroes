
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
			foreach (T item in (List<T>)val) {
				yield return item;
			}
		}
	}

	/// <summary> Adds the Object to the cache. </summary>
	public void Add<T>(T obj) where T : TObj {
		var type = obj.GetType();
		while (true) {
			Type listType = typeof(List<>).MakeGenericType(new[] { type });
			if (!dict.TryGetValue(type, out var list)) {
				dict[type] = list = Activator.CreateInstance(listType);
			}
			var add = listType.GetMethod("Add");
			add.Invoke(list, new object[] { obj });
#if DEBUG
			if (obj is Unit) {
				dynamic dynList = list;
				var total = 0;
				foreach (var item in dynList) {
					if (item == obj) {
						total++;
					}
				}
				if (total > 1) {
					Debug.Log($"Added {obj} to {type.Name}. There is now {total} copies of it???");
				}
			}
#endif
			if (type == typeof(TObj)) break;
			type = type.BaseType;
		}
	}

	/// <summary> Removes the Object from the cache. </summary>
	public void Remove(TObj obj) {
		var type = obj.GetType();
		while (true) {
			Type listType = typeof(List<>).MakeGenericType(new[] { type });
			if (dict.TryGetValue(type, out var list)) {
				var remove = listType.GetMethod("Remove");
				remove.Invoke(list, new object[] { obj });
			} else {
				Debug.LogWarning($"Couldn't remove {obj} from {type.Name} because the list for that type was missing!");
			}
			if (type == typeof(TObj)) break;
			type = type.BaseType;
		}
	}

	/// <summary> Returns the first Object of type T. </summary>
	public T First<T>() where T : TObj {
		var type = typeof(T);
		if (dict.TryGetValue(type, out var val)) {
			var res = ((List<T>)val).FirstOrDefault();
			if (res != null) return (T)res;
		}
		throw new InvalidOperationException($"No {nameof(Modifier)} of type {typeof(T).Name}.");
	}
	/// <summary> Returns the first Object of type T or null if none exist. </summary>
	public T FirstOrDefault<T>() where T : TObj {
		var type = typeof(T);
		if (dict.TryGetValue(type, out var val)) {
			var res = ((List<T>)val).FirstOrDefault();
			return (T)res;
		}
		return default;
	}

	public int IndexOf<T>(TObj obj) where T : TObj {
		var type = typeof(T);
		if (dict.TryGetValue(type, out var val)) {
			return ((List<T>)val).IndexOf((T)obj);
		}
		return -1;
	}

	/// <summary> Returns whether an Object of type T exists. </summary>
	public bool Contains<T>() where T : TObj {
		var type = typeof(T);
		if (dict.TryGetValue(type, out var val)) {
			var list = (List<T>)val;
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

	[SerializeField] string[] keys;
	[SerializeField] ObjectArrayContainer[] vals;

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
			var add = listType.GetMethod("Add");
			foreach (var comp in comps) {
				add.Invoke(list, new object[] { comp });
			}
		}
		keys = null;
		vals = null;
	}

	#endregion
}
