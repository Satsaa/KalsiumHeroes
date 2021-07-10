
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.Events;

public class LangTextSetter : ValueReceiver<DataObject, DataObjectData> {

	[SerializeField, Tooltip("String added to the identifier of the received DataObjectData which is then translated.")]
	string strIdSuffix = "DisplayName";

	[SerializeField]
	UnityEvent<string> onUpdate;

	protected override void ReceiveValue(DataObjectData data) {
		onUpdate.Invoke(Lang.GetStr($"{data.identifier}_{strIdSuffix}"));
	}

	protected override void ReceiveValue(DataObject obj) {
		ReceiveValue(obj.data);
	}

}