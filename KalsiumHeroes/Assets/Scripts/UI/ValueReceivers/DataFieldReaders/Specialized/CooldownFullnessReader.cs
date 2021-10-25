
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.Events;

public class CooldownFullnessReader : ValueReceiver<UnitModifier>, ICustomOnConfigureNonPersistent {

	[SerializeField] protected NumericDataFieldSelector cooldownSelector;
	[SerializeField] protected NumericDataFieldSelector chargesSelector;

	[SerializeField] bool inverse;

	[SerializeField] UnityEvent<float> onUpdate;

	[SerializeField, HideInInspector] protected UnitModifier data;
	[SerializeField, HideInInspector] bool listenered;

	protected virtual void Awake() => OnConfigureNonpersistent(true);
	protected virtual void OnDestroy() => OnConfigureNonpersistent(false);
	protected sealed override void ReceiveValue(UnitModifier target) => Setup(target);


	protected virtual void Handle() {
		if (!data) return;
		var value = cooldownSelector.GetValue(data) / cooldownSelector.GetOther(data);
		if (chargesSelector.GetValue(data) == chargesSelector.GetOther(data)) value = 0;
		if (float.IsNaN(value)) value = 1;
		if (inverse) value = 1 - value;
		onUpdate.Invoke(value);
	}

	private void Setup(UnitModifier data) {
		TryConfigureListeners(false);
		this.data = data;
		TryConfigureListeners(true);
		Handle();
	}

	public virtual void OnConfigureNonpersistent(bool add) {
		TryConfigureListeners(add);
	}

	protected void TryConfigureListeners(bool add) {

		if (chargesSelector != null && data != null) {
			if (listenered != add) {
				listenered = add;
				var att = chargesSelector.GetFieldValue(data);

				if (att is Attribute<int> a) {
					a.current.onChanged.ConfigureListener(add, () => Handle());
					if (a.count > 1) {
						a.values[1].onChanged.ConfigureListener(add, () => Handle());
					}
				}

			}
		}

		if (cooldownSelector != null && data != null) {
			if (listenered != add) {
				listenered = add;
				var att = cooldownSelector.GetFieldValue(data);

				if (att is Attribute<int> a) {
					a.current.onChanged.ConfigureListener(add, () => Handle());
					if (a.count > 1) {
						a.values[1].onChanged.ConfigureListener(add, () => Handle());
					}
				}

			}
		}

	}

}