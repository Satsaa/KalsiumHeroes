
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Extensions;
using UnityEngine.Events;

public class EnabledReader : DataFieldReader<bool> {

	[SerializeField] Color onColor = Color.green;
	[SerializeField] Color offColor = Color.red;

	[SerializeField] UnityEvent<Color> onUpdate;

	protected override void OnReceive() => Handle();
	protected override void OnValue(bool value) => Handle();
	protected override void OnOther(bool value) => Handle();
	protected override void OnEnabled(bool enabled) => Handle();

	protected virtual void Handle() {

		var current = selector.GetValue(data);

		onUpdate.Invoke(current ? onColor : offColor);

	}

}