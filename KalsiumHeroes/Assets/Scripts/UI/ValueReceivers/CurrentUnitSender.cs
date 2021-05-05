
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class CurrentUnitSender : Hooker, IOnTurnStart_Global {

	public void OnTurnStart(Unit unit) {
		ValueReceiver.SendValue(gameObject, unit);
	}

}
