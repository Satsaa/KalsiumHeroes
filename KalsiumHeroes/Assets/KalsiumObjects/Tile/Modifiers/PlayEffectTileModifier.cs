using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = nameof(PlayEffectTileModifier), menuName = "KalsiumHeroes/TileModifier/" + nameof(PlayEffectTileModifier))]
public class PlayEffectTileModifier : TileModifier, IOnMoveOn_Tile {

	public void OnMoveOn(Modifier reason, Unit unit) {
		if (!container) return;
		var vfx = container.GetComponent<VisualEffect>();
		if (vfx) vfx.Play();
		var pts = container.GetComponent<ParticleSystem>();
		if (pts) pts.Play();
	}
}
