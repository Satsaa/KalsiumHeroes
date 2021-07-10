
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.Events;
using Muc.Data;

public class AbilityTypeReader : EnumReader<AbilityType> {

	protected override void Handle() {
		if (data is PassiveData) {
			onUpdate.Invoke(Lang.GetStr("ABILITY_TYPE_PASSIVE"));
		} else {
			base.Handle();
		}
	}

}