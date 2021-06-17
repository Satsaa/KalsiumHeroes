
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class BoolReader : DataFieldReader<bool> {

	protected override void OnReceive() { }
	protected override void OnValue(bool value) { }
	protected override void OnOther(bool value) { }
	protected override void OnEnabled(bool enabled) { }

}