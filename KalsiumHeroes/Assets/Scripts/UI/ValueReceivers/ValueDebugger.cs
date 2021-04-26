
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Data;

public class ValueDebugger : ValueReceiver<object> {

	protected override void ReceiveValue(object value) {
		Debug.Log($"Received value of type {value.GetType()}: {value}", this);
	}
}
