
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Systems.RenderImages;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class IntSetter : ValueReceiver<int> {

	protected override void ReceiveValue(int data) {
		GetComponent<TMPro.TMP_Text>().text = data.ToString();
	}

}