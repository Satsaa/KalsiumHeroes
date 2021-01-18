
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Collections;
using Object = UnityEngine.Object;

/// <summary>
/// Stores IOnEvents in collections based on their types.
/// </summary>
[Serializable]
public class OnEventDict : OnEventDict<IOnEvent>, ISerializationCallbackReceiver { }

/// <summary>
/// Stores IOnEvents in collections based on their types.
/// </summary>
[Serializable]
public class OnEventDict<TBase> : ISerializationCallbackReceiver where TBase : IOnEvent {

	Dictionary<Type, object> dict = new Dictionary<Type, object>();
	public static int executing = 0;

	/// <summary> Enumerates IOnEvents of type T. </summary>
	public IEnumerable<T> Get<T>() where T : TBase {
		if (dict.TryGetValue(typeof(T), out var val)) {
			return val as HashSet<T>;
		} else {
			return new T[0];
		}
	}

	/// <summary> Executes action on IOnEvents of type T. </summary>
	public void Execute<T>(Action<T> action) where T : TBase {
		if (executing > 0) {
			Game.onAfterEvent += () => Execute<T>(action);
			return;
		}
		if (dict.TryGetValue(typeof(T), out var val)) {
			executing++;
			try {
				var targets = val as HashSet<T>;
				foreach (var target in targets) {
					action(target);
				}
			} finally {
				executing--;
				if (executing <= 0) Game.InvokeOnAfterEvent();
			}
		}
	}

	/// <summary> Aggregates on IOnEvents of type T. </summary>
	public TAccumulate Aggregate<T, TAccumulate>(TAccumulate seed, Func<TAccumulate, T, TAccumulate> func) where T : TBase {
		if (dict.TryGetValue(typeof(T), out var val)) {
			executing++;
			try {
				var targets = val as HashSet<T>;
				foreach (var target in targets) {
					seed = func(seed, target);
				}
			} finally {
				executing--;
				if (executing <= 0) Game.InvokeOnAfterEvent();
			}
		}
		return seed;
	}

	/// <summary> Adds the OnEvents to the cache. </summary>
	public void Add(Object obj) {
		if (executing > 0) {
			Game.onAfterEvent += () => Add(obj);
			return;
		}
		foreach (var type in obj.GetType().GetInterfaces()) {
			Type setType = typeof(HashSet<>).MakeGenericType(new[] { type });
			if (!dict.TryGetValue(type, out object set)) {
				dict[type] = set = Activator.CreateInstance(setType);
			}
			var method = setType.GetMethod("Add");
			method.Invoke(set, new object[] { obj });
		}
	}

	/// <summary> Removes the OnEvents from the cache. </summary>
	public void Remove(Object obj) {
		if (executing > 0) {
			Game.onAfterEvent += () => Remove(obj);
			return;
		}
		foreach (var type in obj.GetType().GetInterfaces()) {
			Type setType = typeof(HashSet<>).MakeGenericType(new[] { type });
			var set = dict[type];
			var method = setType.GetMethod("Remove");
			method.Invoke(set, new object[] { obj });
		}
	}


	#region Serialization

	[Serializable]
	private class ObjectArrayContainer {
		public Object[] objs;
		public ObjectArrayContainer(Object[] objs) => this.objs = objs;
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
			var objs = vals[i].objs;
			Type setType = typeof(HashSet<>).MakeGenericType(new[] { type });
			var set = dict[type] = Activator.CreateInstance(setType);
			var method = setType.GetMethod("Add");
			foreach (var obj in objs) {
				method.Invoke(set, new object[] { obj });
			}
		}
		keys = null;
		vals = null;
	}

	#endregion
}