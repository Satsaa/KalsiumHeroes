
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Data;

interface IValueReceiver {
	bool TryHandleValue(object value);
}

public abstract class ValueReceiver : MonoBehaviour, IValueReceiver {

	/// <summary> Sends the value to the GameObject and it't children </summary>
	public static void SendValue(GameObject gameObject, object value, bool onlyChildren = false, bool _isFirst = true) {
		if (!onlyChildren) {
			var receivers = UnityEngine.Pool.ListPool<IValueReceiver>.Get();
			try {
				gameObject.GetComponents<IValueReceiver>(receivers);
				foreach (var receiver in receivers) {
					receiver.TryHandleValue(value);
				}
			} finally {
				UnityEngine.Pool.ListPool<IValueReceiver>.Release(receivers);
			}

			if (_isFirst || !gameObject.TryGetComponent<SendValueStopper>(out var _)) {
				foreach (Transform child in gameObject.transform) {
					SendValue(child.gameObject, value, false, false);
				}
			}
		} else {
			foreach (Transform child in gameObject.transform) {
				SendValue(child.gameObject, value, false, false);
			}
		}
	}

	public abstract bool TryHandleValue(object value);
}

public abstract class ValueReceiver<T> : ValueReceiver {

	[SerializeField, Tooltip("Base type for the first value. You can use this to filter accepted value types.")]
	SerializedType<T> typeConstraint1;

	public virtual IEnumerable<Type> GetParams() {
		yield return typeof(T);
	}

	public override bool TryHandleValue(object value) {
		var handled = value is T && (typeConstraint1.type == null || typeConstraint1.type.IsAssignableFrom(value.GetType()));
		if (handled) ReceiveValue((T)value);
		return handled;
	}

	protected abstract void ReceiveValue(T value);

}

public abstract class ValueReceiver<T1, T2> : ValueReceiver<T1> {

	[SerializeField, Tooltip("Base type for the second value. You can use this to filter accepted value types.")]
	SerializedType<T2> typeConstraint2;

	public override IEnumerable<Type> GetParams() {
		yield return typeof(T1);
		yield return typeof(T2);
	}

	public override bool TryHandleValue(object value) {
		var baseHandled = base.TryHandleValue(value);

		var handled = value is T2 && (typeConstraint2.type == null || typeConstraint2.type.IsAssignableFrom(value.GetType()));
		if (handled) ReceiveValue((T2)value);
		return handled || baseHandled;
	}

	protected abstract void ReceiveValue(T2 value);

}

public abstract class ValueReceiver<T1, T2, T3> : ValueReceiver<T1, T2> {

	[SerializeField, Tooltip("Base type for the third value. You can use this to filter accepted value types.")]
	SerializedType<T3> typeConstraint3;

	public override IEnumerable<Type> GetParams() {
		yield return typeof(T1);
		yield return typeof(T2);
		yield return typeof(T3);
	}

	public override bool TryHandleValue(object value) {
		var baseHandled = base.TryHandleValue(value);

		var handled = value is T3 && (typeConstraint3.type == null || typeConstraint3.type.IsAssignableFrom(value.GetType()));
		if (handled) ReceiveValue((T3)value);
		return handled || baseHandled;
	}

	protected abstract void ReceiveValue(T3 value);

}

public abstract class ValueReceiver<T1, T2, T3, T4> : ValueReceiver<T1, T2, T3> {

	[SerializeField, Tooltip("Base type for the fourth value. You can use this to filter accepted value types.")]
	SerializedType<T4> typeConstraint4;

	public override IEnumerable<Type> GetParams() {
		yield return typeof(T1);
		yield return typeof(T2);
		yield return typeof(T3);
		yield return typeof(T4);
	}

	public override bool TryHandleValue(object value) {
		var baseHandled = base.TryHandleValue(value);

		var handled = value is T4 && (typeConstraint4.type == null || typeConstraint4.type.IsAssignableFrom(value.GetType()));
		if (handled) ReceiveValue((T4)value);
		return handled || baseHandled;
	}

	protected abstract void ReceiveValue(T4 value);

}


public abstract class ValueReceiver<T1, T2, T3, T4, T5> : ValueReceiver<T1, T2, T3, T4> {

	[SerializeField, Tooltip("Base type for the fifth value. You can use this to filter accepted value types.")]
	SerializedType<T5> typeConstraint5;

	public override IEnumerable<Type> GetParams() {
		yield return typeof(T1);
		yield return typeof(T2);
		yield return typeof(T3);
		yield return typeof(T4);
		yield return typeof(T5);
	}

	public override bool TryHandleValue(object value) {
		var baseHandled = base.TryHandleValue(value);

		var handled = value is T5 && (typeConstraint5.type == null || typeConstraint5.type.IsAssignableFrom(value.GetType()));
		if (handled) ReceiveValue((T5)value);
		return handled || baseHandled;
	}

	protected abstract void ReceiveValue(T5 value);

}

#if UNITY_EDITOR
namespace Editors {

	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using Object = UnityEngine.Object;
	using static Muc.Editor.PropertyUtil;
	using static Muc.Editor.EditorUtil;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(ValueReceiver<>), true)]
	public class ValueReceiverTEditor : Editor {

		ValueReceiver t => (ValueReceiver)target;
		dynamic dynT => target;

		public override void OnInspectorGUI() {

			if (targets.Length == 1) {
				EditorGUILayout.LabelField($"ValueReceiver<{String.Join(", ", ((IEnumerable<Type>)dynT.GetParams()).Select(v => v.Name))}>");
			}

			DrawDefaultInspector();

		}
	}
}
#endif