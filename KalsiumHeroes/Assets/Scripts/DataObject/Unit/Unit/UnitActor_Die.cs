

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Muc.Extensions;
using Muc.Geometry;
using System.Linq;

public partial class UnitActor {

	public void Die() {
		animator.SetTrigger("Die");
	}

}
