
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.Events;

public class ChargesShower : ValueReceiver<UnitModifier>, ICustomOnConfigureNonPersistent {

	[SerializeField] protected DataFieldSelector<int> chargesSelector;

	[SerializeField] UnityEvent<bool> onUpdate;

	[SerializeField, HideInInspector] protected UnitModifier data;
	[SerializeField, HideInInspector] bool listenered;

	protected virtual void Awake() => OnConfigureNonpersistent(true);
	protected virtual void OnDestroy() => OnConfigureNonpersistent(false);
	protected sealed override void ReceiveValue(UnitModifier target) => Setup(target);


	protected virtual void Handle() {
		if (!data) return;
		var value = chargesSelector.GetValue(data);
		var other = chargesSelector.GetOther(data);
		if (value <= 1 && other == 1) {
			onUpdate.Invoke(false);
			return;
		}
		onUpdate.Invoke(true);
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

	}

}