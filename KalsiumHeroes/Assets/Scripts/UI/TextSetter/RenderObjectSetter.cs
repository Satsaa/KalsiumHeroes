
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Systems.RenderImages;

[RequireComponent(typeof(RenderImage))]
public class RenderObjectSetter : ValueReceiver<UnitData> {

	protected override void ReceiveValue(UnitData data) {
		GetComponent<RenderImage>().SetRenderObject(data.preview);
	}

}