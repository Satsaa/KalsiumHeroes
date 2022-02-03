
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class GameModifier : GameMaster.RootModifier {

	public GameMaster game => master;

	protected override void OnCreate() {
		//
		base.OnCreate();
	}

	protected override void OnRemove() {
		//
		base.OnRemove();
	}

}

