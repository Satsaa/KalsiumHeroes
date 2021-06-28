
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.Events;

public class LangTextSetter : ValueReceiver<DataObject, DataObjectData> {

	[SerializeField, Tooltip("String added to the identifier of the received DataObjectData which is then translated.")]
	string msgIdSuffix = "DISPLAY_NAME";

	[SerializeField]
	UnityEvent<string> onUpdate;

	protected override void ReceiveValue(DataObjectData data) {
		onUpdate.Invoke(Lang.GetText($"{data.identifier} {msgIdSuffix}"));
	}

	protected override void ReceiveValue(DataObject obj) {
		ReceiveValue(obj.data);
	}

}