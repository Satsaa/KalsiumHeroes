
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.Events;

public class LangTextSetter : ValueReceiver<KalsiumObject> {

	[SerializeField, Tooltip("String added to the identifier of the received KalsiumObjectData which is then translated.")]
	string strIdSuffix = "DisplayName";

	[SerializeField]
	UnityEvent<string> onUpdate;

	protected override void ReceiveValue(KalsiumObject obj) {
		onUpdate.Invoke(Lang.GetStr($"{obj.identifier}_{strIdSuffix}"));
	}

}