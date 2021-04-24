
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public abstract class ValueReceiver : MonoBehaviour {

	protected bool received = false;

	/// <summary> Sends the value to the GameObject and it't children </summary>
	public static void SendValue(GameObject gameObject, object value, bool onlyChildren = false, bool _isFirst = true) {
		if (!onlyChildren) {
			var receivers = UnityEngine.Pool.ListPool<ValueReceiver>.Get();
			try {
				gameObject.GetComponents<ValueReceiver>(receivers);
				foreach (var receiver in receivers) {
					if (receiver.IsValid(value)) {
						receiver.GiveValue(value);
					}
				}
			} finally {
				UnityEngine.Pool.ListPool<ValueReceiver>.Release(receivers);
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

	public bool IsValid(object value) => (received != (received = true)) && Validate(value);
	protected abstract bool Validate(object value);
	public virtual void GiveValue(object value) { }
}

public abstract class ValueReceiver<T> : ValueReceiver {

	protected override bool Validate(object value) {
		return value is T;
	}

	public override void GiveValue(object value) {
		ReceiveValue((T)value);
	}

	protected abstract void ReceiveValue(T value);

}

public abstract class ValueReceiver<T1, T2> : ValueReceiver<T1> {

	protected override bool Validate(object value) {
		return base.Validate(value) || value is T2;
	}

	public override void GiveValue(object value) {
		switch (value) {
			case T1 t1:
				ReceiveValue(t1);
				break;
			case T2 t2:
				ReceiveValue(t2);
				break;
			default:
				throw new ArgumentException("Invalid type", nameof(value));
		}

	}

	protected abstract void ReceiveValue(T2 value);

}

public abstract class ValueReceiver<T1, T2, T3> : ValueReceiver<T1, T2> {

	protected override bool Validate(object value) {
		return base.Validate(value) || value is T3;
	}

	public override void GiveValue(object value) {
		switch (value) {
			case T1 t1:
				ReceiveValue(t1);
				break;
			case T2 t2:
				ReceiveValue(t2);
				break;
			case T3 t3:
				ReceiveValue(t3);
				break;
			default:
				throw new ArgumentException("Invalid type", nameof(value));
		}

	}

	protected abstract void ReceiveValue(T3 value);

}

public abstract class ValueReceiver<T1, T2, T3, T4> : ValueReceiver<T1, T2, T3> {

	protected override bool Validate(object value) {
		return base.Validate(value) || value is T4;
	}

	public override void GiveValue(object value) {
		switch (value) {
			case T1 t1:
				ReceiveValue(t1);
				break;
			case T2 t2:
				ReceiveValue(t2);
				break;
			case T3 t3:
				ReceiveValue(t3);
				break;
			case T4 t4:
				ReceiveValue(t4);
				break;
			default:
				throw new ArgumentException("Invalid type", nameof(value));
		}

	}

	protected abstract void ReceiveValue(T4 value);

}


public abstract class ValueReceiver<T1, T2, T3, T4, T5> : ValueReceiver<T1, T2, T3, T4> {

	protected override bool Validate(object value) {
		return base.Validate(value) || value is T5;
	}

	public override void GiveValue(object value) {
		switch (value) {
			case T1 t1:
				ReceiveValue(t1);
				break;
			case T2 t2:
				ReceiveValue(t2);
				break;
			case T3 t3:
				ReceiveValue(t3);
				break;
			case T4 t4:
				ReceiveValue(t4);
				break;
			case T5 t5:
				ReceiveValue(t5);
				break;
			default:
				throw new ArgumentException("Invalid type", nameof(value));
		}

	}

	protected abstract void ReceiveValue(T5 value);

}

