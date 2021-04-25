
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.UI;
using IHas;

[RequireComponent(typeof(Image))]
public class SpriteSetter : ValueReceiver<IHasSprite> {

	protected override void ReceiveValue(IHasSprite obj) {
		GetComponent<Image>().sprite = obj.sprite;
	}

}