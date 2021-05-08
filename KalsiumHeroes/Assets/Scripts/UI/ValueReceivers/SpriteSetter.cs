
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.UI;
using IHas;

[RequireComponent(typeof(Image))]
public class SpriteSetter : ValueReceiver<IHasSprite, DataObject> {

	protected override void ReceiveValue(IHasSprite obj) {
		GetComponent<Image>().sprite = obj.sprite;
	}

	protected override void ReceiveValue(DataObject obj) {
		if (obj.data is IHasSprite hasSprite) {
			GetComponent<Image>().sprite = hasSprite.sprite;
		}
	}

}