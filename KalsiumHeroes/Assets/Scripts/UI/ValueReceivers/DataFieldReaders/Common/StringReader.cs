
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class StringReader : DataFieldReader<string> {

	protected override void OnReceive() { }
	protected override void OnValue(string value) { }
	protected override void OnOther(string value) { }
	protected override void OnEnabled(bool enabled) { }

}