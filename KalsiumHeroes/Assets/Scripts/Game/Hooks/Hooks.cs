
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Collections;
using Object = UnityEngine.Object;
using Muc;
using Muc.Collections;

public abstract class Hooks {

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

	/// <summary> This event is Invoked ONCE after the execution of IHooks in the current scope ends. </summary>
	public static event Action onFinishEvent {
		remove { if (current == null || !current.active) throw new InvalidOperationException("No active scope."); else current.onFinish -= value; }
		add { if (current == null || !current.active) throw new InvalidOperationException("No active scope."); else current.onFinish += value; }
	}


	/// <summary> Adds an OnEvents to the cache. </summary>
	public abstract void Hook(Object obj);

	/// <summary> Removes an OnEvents from the cache. </summary>
	public abstract void Unhook(Object obj);

}

/// <summary>
/// Stores IHooks in collections based on their types.
/// </summary>
[Serializable]
public class Hooks<TBase> : Hooks, ISerializationCallbackReceiver where TBase : IHook {

	Dictionary<Type, object> dict = new Dictionary<Type, object>();


	/// <summary> Returns IHooks of type T inside a new List. </summary>
	public IEnumerable<T> Get<T>() where T : TBase {
		if (dict.TryGetValue(typeof(T), out var val)) {
			return new List<T>(val as SafeList<T>);
		} else {
			return new List<T>();
		}
	}

	/// <summary> Invokes action on all IHooks of type T. </summary>
	public void ForEach<T>(Scope scope, Action<T> action) where T : TBase {
		if (dict.TryGetValue(typeof(T), out var val)) {
			var list = val as SafeList<T>;
			foreach (var v in list) {
				current = scope;
				action(v);
			}
		}
	}

	public override void Hook(Object obj) {
		foreach (var type in obj.GetType().GetInterfaces()) {
			Type listType = typeof(SafeList<>).MakeGenericType(new[] { type });
			if (!dict.TryGetValue(type, out object list)) {
				dict[type] = list = Activator.CreateInstance(listType);
			}
			var add = listType.GetMethod("Add");
			add.Invoke(list, new object[] { obj });
#if DEBUG // Ensure no duplicates are created
			var dynList = list as IEnumerable;
			var total = 0;
			foreach (var item in dynList) {
				if (obj.Equals(item)) total++;
			}
			if (total > 1) Debug.Log($"Added {obj} to {type.Name}. ${total} duplicates exist!");
#endif
		}
	}

	public override void Unhook(Object obj) {
		foreach (var type in obj.GetType().GetInterfaces()) {
			Type listType = typeof(SafeList<>).MakeGenericType(new[] { type });
			if (dict.TryGetValue(type, out var list)) {
				var remove = listType.GetMethod("Remove");
				remove.Invoke(list, new object[] { obj });
			} else {
				Debug.LogWarning($"Couldn't remove {obj} from {type.Name} because the list for that type was missing!");
			}
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
			Type listType = typeof(SafeList<>).MakeGenericType(new[] { type });
			var list = dict[type] = Activator.CreateInstance(listType);
			var add = listType.GetMethod("Add");
			foreach (var obj in objs) {
				add.Invoke(list, new object[] { obj });
			}
		}
		keys = null;
		vals = null;
	}

	#endregion
}