
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class TypeNameSetter : ValueReceiver<object> {

	protected override void ReceiveValue(object obj) {
		GetComponent<TMPro.TMP_Text>().text = obj.GetType().ToString();
	}

}