
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
	private bool executing = false;

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
		if (executing) {
			Game.onAfterEvent += () => {
				Execute<T>(action);
			};
		}
		executing = true;
		if (dict.TryGetValue(typeof(T), out var val)) {
			try {
				var targets = val as HashSet<T>;
				foreach (var target in targets) {
					action(target);
				}
			} finally {
				executing = false;
				Game.InvokeOnAfterEvent();
			}
		}
		executing = false;
		Game.InvokeOnAfterEvent();
	}

	/// <summary> Aggregates on IOnEvents of type T. </summary>
	public TAccumulate Aggregate<T, TAccumulate>(TAccumulate seed, Func<TAccumulate, T, TAccumulate> func) where T : TBase {
		executing = true;
		if (dict.TryGetValue(typeof(T), out var val)) {
			try {
				var targets = val as HashSet<T>;
				foreach (var target in targets) {
					seed = func(seed, target);
				}
			} finally {
				executing = false;
				Game.InvokeOnAfterEvent();
			}
		}
		executing = false;
		Game.InvokeOnAfterEvent();
		return seed;
	}

	/// <summary> Adds the OnEvents to the cache. </summary>
	public void Add(Object obj) {
		if (executing) {
			Game.onAfterEvent += () => Add(obj); ;
			return;
		}
		foreach (var type in obj.GetType().GetInterfaces()) {
			Type setType = typeof(HashSet<>).MakeGenericType(new[] { type });
			if (!dict.TryGetValue(type, out dynamic set)) {
				dict[type] = set = Activator.CreateInstance(setType);
			}
			set.Add(obj as dynamic);
		}
	}

	/// <summary> Removes the OnEvents from the cache. </summary>
	public void Remove(Object obj) {
		foreach (var type in obj.GetType().GetInterfaces()) {
			Type setType = typeof(HashSet<>).MakeGenericType(new[] { type });
			dynamic set = dict[type];
			set.Remove(obj as dynamic);
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
			dynamic set = dict[type] = Activator.CreateInstance(setType);
			var method = setType.GetMethod("Add");
			foreach (dynamic obj in objs) {
				set.Add(obj);
			}
		}
		keys = null;
		vals = null;
	}

	#endregion
}