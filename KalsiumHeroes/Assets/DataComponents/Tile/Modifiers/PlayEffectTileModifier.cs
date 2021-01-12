using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

public class PlayEffectTileModifier : TileModifier, IOnMoveOn_Tile {

	public void OnMoveOn(Unit unit) {
		var vfx = GetComponent<VisualEffect>();
		if (vfx) vfx.Play();
		var pts = GetComponent<ParticleSystem>();
		if (pts) pts.Play();
	}
}
