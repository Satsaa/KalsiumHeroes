using UnityEngine;
using UnityEngine.Events;

public abstract class NumericDataFieldReader : ValueReceiver<DataObject, DataObjectData>, ICustomOnConfigureNonPersistent {

	[SerializeField] protected NumericDataFieldSelector selector;

	[SerializeField] UnityEvent<float> onValue;
	[SerializeField] UnityEvent<float> onOther;
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
	protected abstract void OnValue(float value);
	protected abstract void OnOther(float value);
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

				switch (att) {
					case Attribute<float> f:
						f.onValueChanged.ConfigureListener(add, () => {
							var param = selector.GetValue(data);
							OnValue(param);
							onValue.Invoke(param);
						});
						break;
					case Attribute<int> i:
						i.onValueChanged.ConfigureListener(add, () => {
							var param = selector.GetValue(data);
							OnValue(param);
							onValue.Invoke(param);
						});
						break;
				}

				switch (att) {
					case DualAttribute<float> f:
						f.onOtherChanged.ConfigureListener(add, () => {
							var param = selector.GetOther(data);
							OnOther(param);
							onOther.Invoke(param);
						});
						break;
					case DualAttribute<int> i:
						i.onOtherChanged.ConfigureListener(add, () => {
							var param = selector.GetOther(data);
							OnOther(param);
							onOther.Invoke(param);
						});
						break;
				}

				switch (att) {
					case ToggleAttribute<float> f:
						f.onEnabledChanged.ConfigureListener(add, () => {
							var param = selector.GetEnabled(data);
							OnEnabled(param);
							onEnabled.Invoke(param);
						});
						break;
					case ToggleDualAttribute<float> df:
						df.onEnabledChanged.ConfigureListener(add, () => {
							var param = selector.GetEnabled(data);
							OnEnabled(param);
							onEnabled.Invoke(param);
						});
						break;
					case ToggleAttribute<int> i:
						i.onEnabledChanged.ConfigureListener(add, () => {
							var param = selector.GetEnabled(data);
							OnEnabled(param);
							onEnabled.Invoke(param);
						});
						break;
					case ToggleDualAttribute<int> di:
						di.onEnabledChanged.ConfigureListener(add, () => {
							var param = selector.GetEnabled(data);
							OnEnabled(param);
							onEnabled.Invoke(param);
						});
						break;
				}
			}
		}
	}

}
