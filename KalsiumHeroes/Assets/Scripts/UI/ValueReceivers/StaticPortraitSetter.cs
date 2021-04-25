
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class StaticPortraitSetter : ValueReceiver<UnitData> {

	protected override void ReceiveValue(UnitData data) {
		GetComponent<Image>().sprite = data.staticPortrait;
	}

}