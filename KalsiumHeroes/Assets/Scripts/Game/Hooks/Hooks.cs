
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Collections;
using Object = UnityEngine.Object;
using Muc;
using Muc.Collections;
using Muc.Extensions;

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


	public class HooksList<T> : SafeList<T> {

		public List<int> orders = new();

		public void AddOrdered(Type hookerType, Type hookType, T item) {
			var order = HookOrders.instance.GetOrder(hookerType, hookType);
			for (int i = 0; i < orders.Count; i++) {
				var current = orders[i];
				if (current > order) {
					orders.Insert(i, order);
					Insert(i, item);
					return;
				}
			}
			orders.Add(order);
			Add(item);
		}

		public void RemoveOrdered(T item) {
			var index = IndexOf(item);
			RemoveAt(index);
			orders.RemoveAt(index);
		}

	}
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
				action(v); // 6
			}
		}
	}

	public override void Hook(Object obj) {
		var hookerType = obj.GetType();
		foreach (var iType in obj.GetType().GetInterfaces().Where(v => v.GetInterfaces().Contains(typeof(IHook)))) {
			Type listType = typeof(HooksList<>).MakeGenericType(new[] { iType });
			if (!dict.TryGetValue(iType, out object list)) {
				dict[iType] = list = Activator.CreateInstance(listType);
			}
			var add = listType.GetMethod(nameof(HooksList<int>.AddOrdered));
			add.Invoke(list, new object[] { hookerType, iType, obj });
#if DEBUG // Ensure no duplicates are created
			var dynList = list as IEnumerable;
			var total = 0;
			foreach (var item in dynList) {
				if (obj.Equals(item)) total++;
			}
			if (total > 1) Debug.Log($"Added {obj} to {iType.Name}. ${total} duplicates exist!");
#endif
		}
	}

	public override void Unhook(Object obj) {
		foreach (var iType in obj.GetType().GetInterfaces().Where(v => v.GetInterfaces().Contains(typeof(IHook)))) {
			Type listType = typeof(HooksList<>).MakeGenericType(new[] { iType });
			if (dict.TryGetValue(iType, out var list)) {
				var remove = listType.GetMethod(nameof(HooksList<int>.RemoveOrdered));
				remove.Invoke(list, new object[] { obj });
			} else {
				Debug.LogWarning($"Couldn't remove {obj} from {iType.Name} because the list for that type was missing!");
			}
		}
	}


	#region Serialization

	[Serializable]
	private class ObjectArrayContainer {
		public Object[] objs;
		public ObjectArrayContainer(Object[] objs) => this.objs = objs;
	}

	[Serializable]
	private class OrderContainer {
		public List<int> orders;
		public OrderContainer(List<int> orders) => this.orders = orders;
	}

	[SerializeField, HideInInspector] List<string> keys;
	[SerializeField, HideInInspector] List<ObjectArrayContainer> vals;
	[SerializeField, HideInInspector] List<OrderContainer> ordahs;

	void ISerializationCallbackReceiver.OnBeforeSerialize() {
		keys = dict.Keys.Select(v => v.GetShortQualifiedName()).ToList();
		vals = dict.Values.Select(v => (v as IEnumerable).Cast<Object>().ToArray()).Select(v => new ObjectArrayContainer(v)).ToList();
		ordahs = dict.Values.Select(v => new OrderContainer(v.GetType().GetField(nameof(HooksList<int>.orders)).GetValue(v) as List<int>)).ToList();
	}

	void ISerializationCallbackReceiver.OnAfterDeserialize() {
		for (int i = 0; i < keys.Count; i++) {
			var type = Type.GetType(keys[i]);
			var objs = vals[i].objs;
			var orders = ordahs[i].orders;
			Type listType = typeof(HooksList<>).MakeGenericType(new[] { type });
			var list = dict[type] = Activator.CreateInstance(listType);
			var add = listType.GetMethod(nameof(HooksList<int>.Add));

			for (int j = 0; j < objs.Length; j++) {
				var obj = objs[j];
				add.Invoke(list, new object[] { obj });
			}

			listType.GetField(nameof(HooksList<int>.orders)).SetValue(list, orders);
		}
		keys = null;
		vals = null;
		ordahs = null;
	}

	#endregion

}