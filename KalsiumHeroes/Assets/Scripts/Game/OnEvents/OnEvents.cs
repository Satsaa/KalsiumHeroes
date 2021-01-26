
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Collections;
using Object = UnityEngine.Object;
using Muc;

public class OnEvents {

	protected class SharedList<TBase> where TBase : IOnEvent {

		/// <summary>
		/// Shared list used when iterating to reduce allocations.
		/// If the list contains items it is in use and a dedicated list should be used instead.
		/// </summary>
		public static List<TBase> list = new List<TBase>();
	}

	public class Scope : IDisposable {
		public Scope() => active = true;

		public Action onFinish;
		public bool active;

		public void Dispose() {
			if (!active) throw new InvalidOperationException("The scope has already been disposed.");
			onFinish?.Invoke();
			current = null;
			active = false;
		}
	}

	protected static Scope current;

	public static bool executing => current != null;

	/// <summary> This event is Invoked ONCE after the execution of IOnEvents in the current scope ends. </summary>
	public static event Action onFinishEvent {
		remove { if (current == null || !current.active) throw new InvalidOperationException("No active scope."); else current.onFinish -= value; }
		add { if (current == null || !current.active) throw new InvalidOperationException("No active scope."); else current.onFinish += value; }
	}

}

/// <summary>
/// Stores IOnEvents in collections based on their types.
/// </summary>
[Serializable]
public class OnEvents<TBase> : OnEvents, ISerializationCallbackReceiver where TBase : IOnEvent {

	Dictionary<Type, object> dict = new Dictionary<Type, object>();


	/// <summary> Returns IOnEvents of type T inside a new List. </summary>
	public IEnumerable<T> Get<T>() where T : TBase {
		if (dict.TryGetValue(typeof(T), out var val)) {
			return new List<T>(val as List<T>);
		} else {
			return new List<T>();
		}
	}

	/// <summary> Invokes action on all IOnEvents of type T. </summary>
	public void ForEach<T>(Scope scope, Action<T> action) where T : TBase {
		if (dict.TryGetValue(typeof(T), out var val)) {
			var list = val as List<T>;
			var target = SharedList<T>.list;
			var usingShared = target.Count == 0;
			if (usingShared) {
				target.AddRange(list);
			} else {
				target = new List<T>(list);
			}
			try {
				foreach (var v in target) {
					current = scope;
					action(v);
				}
			} finally {
				// Don't bother clearing if it was created on the spot.
				if (usingShared) target.Clear();
			}
		}
	}

	/// <summary> Adds the OnEvents to the cache. </summary>
	public void Add(Object obj) {
		foreach (var type in obj.GetType().GetInterfaces()) {
			Type listType = typeof(List<>).MakeGenericType(new[] { type });
			if (!dict.TryGetValue(type, out object list)) {
				dict[type] = list = Activator.CreateInstance(listType);
			}
			var method = listType.GetMethod("Add");
			method.Invoke(list, new object[] { obj });
		}
	}

	/// <summary> Removes the OnEvents from the cache. </summary>
	public void Remove(Object obj) {
		foreach (var type in obj.GetType().GetInterfaces()) {
			Type listType = typeof(List<>).MakeGenericType(new[] { type });
			var list = dict[type];
			var method = listType.GetMethod("Remove");
			method.Invoke(list, new object[] { obj });
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
			Type listType = typeof(List<>).MakeGenericType(new[] { type });
			var list = dict[type] = Activator.CreateInstance(listType);
			var method = listType.GetMethod("Add");
			foreach (var obj in objs) {
				method.Invoke(list, new object[] { obj });
			}
		}
		keys = null;
		vals = null;
	}

	#endregion
}