
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Systems.RenderImages;

public class RenderObjectReader : DataFieldReader<RenderObject> {

	protected override void OnReceive() { }
	protected override void OnValue(RenderObject value) { }
	protected override void OnOther(RenderObject value) { }
	protected override void OnEnabled(bool enabled) { }

}