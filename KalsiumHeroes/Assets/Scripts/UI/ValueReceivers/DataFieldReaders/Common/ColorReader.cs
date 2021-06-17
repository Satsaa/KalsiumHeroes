
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class ColorReader : DataFieldReader<Color> {

	protected override void OnReceive() { }
	protected override void OnValue(Color value) { }
	protected override void OnOther(Color value) { }
	protected override void OnEnabled(bool enabled) { }

}