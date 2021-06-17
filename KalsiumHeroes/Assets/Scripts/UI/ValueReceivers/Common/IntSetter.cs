
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.UI;
using Muc.Data;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class IntSetter : ValueReceiver<int> {

	protected override void ReceiveValue(int value) {
		GetComponent<TMPro.TMP_Text>().text = value.ToString();
	}

}