using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

public class PlayEffectTileModifier : TileModifier {

	public override void OnMoveOn(Unit unit) {
		base.OnMoveOn(unit);
		var vfx = GetComponent<VisualEffect>();
		if (vfx) vfx.Play();
		var pts = GetComponent<ParticleSystem>();
		if (pts) pts.Play();
	}
}
