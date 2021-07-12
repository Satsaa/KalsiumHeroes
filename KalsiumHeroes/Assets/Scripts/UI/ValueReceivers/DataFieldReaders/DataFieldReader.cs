
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Data;
using UnityEngine.Events;

public abstract class DataFieldReader<T> : ValueReceiver<DataObject, DataObjectData>, ICustomOnConfigureNonPersistent {

	[SerializeField] protected DataFieldSelector<T> selector;

	[SerializeField] UnityEvent<T> onValue;
	[SerializeField] UnityEvent<T> onOther;
	[SerializeField] UnityEvent<bool> onEnabled;

	[SerializeField, HideInInspector] protected DataObjectData data;
	[SerializeField, HideInInspector] bool listenered;

	protected virtual void Awake() {
		OnConfigureNonpersistent(true);
	}

	protected virtual void OnDestroy() {
		OnConfigureNonpersistent(false);
	}

	protected abstract void OnReceive();
	protected abstract void OnValue(T value);
	protected abstract void OnOther(T value);
	protected abstract void OnEnabled(bool enabled);

	protected sealed override void ReceiveValue(DataObjectData data) {
		Setup(data);
	}

	protected sealed override void ReceiveValue(DataObject target) {
		Setup(target.data);
	}


	private void Setup(DataObjectData data) {
		TryConfigureListeners(false);
		this.data = data;
		OnReceive();

		var value = selector.GetValue(data);
		OnValue(value);
		onValue.Invoke(value);

		var other = selector.GetOther(data);
		OnOther(other);
		onOther.Invoke(other);

		var enabled = selector.GetEnabled(data);
		OnEnabled(enabled);
		onEnabled.Invoke(enabled);

		TryConfigureListeners(true);
	}

	public virtual void OnConfigureNonpersistent(bool add) {
		TryConfigureListeners(add);
	}

	protected void TryConfigureListeners(bool add) {
		if (selector != null && data != null) {
			if (listenered != add) {
				listenered = add;
				var att = selector.GetFieldValue(data);

				if (att is Attribute<int> a) {
					a.current.onChanged.ConfigureListener(add, () => {
						var param = selector.GetValue(data);
						OnValue(param);
						onValue.Invoke(param);
					});
					if (a.count > 1) {
						a.values[1].onChanged.ConfigureListener(add, () => {
							var param = selector.GetOther(data);
							OnOther(param);
							onOther.Invoke(param);
						});
					}

					var ia = a as IAttribute;
					if (ia.GetEnabled() != null) {
						ia.GetEnabled().onChanged.ConfigureListener(add, () => {
							var param = selector.GetEnabled(data);
							OnEnabled(param);
							onEnabled.Invoke(param);
						});
					}
				}

			}
		}
	}

}
