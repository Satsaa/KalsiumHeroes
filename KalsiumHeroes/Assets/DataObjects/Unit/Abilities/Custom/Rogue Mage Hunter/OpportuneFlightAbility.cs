using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpportuneFlightAbility : Ability, IOnGameStart
{
    public new OpportuneFlightAbilityData data => (OpportuneFlightAbilityData)_data;
    public override Type dataType => typeof(OpportuneFlightAbilityData);

    public override IEnumerable<Tile> GetTargets() {
        return base.GetTargets().Where(v => !v.unit && v.data.passable.value);
    }

    public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability msg) {
        return new InstantAbilityHandler(msg, this, (ability) => {
            var target = Game.grid.tiles[msg.targets.First()];
            var aoe = GetAffectedArea(target);
            foreach (var tile in aoe) {
                unit.MoveTo(GetTargetTile(tile), true);
            }
        });
    }

    Tile GetTargetTile(Tile tile) {
        var dir = unit.tile.GetDir(tile);
        Tile nbr = null;
        int i = 0;
        while ((nbr = tile.GetNeighbor(dir)) != null && !nbr.unit && nbr.data.passable.value && i <= data.moveDistance.value - 2) {
            tile = nbr;
            i++;
        }
        return tile;
    }

    public void OnGameStart() {
        data.cooldown.ResetValue();
    }
}
