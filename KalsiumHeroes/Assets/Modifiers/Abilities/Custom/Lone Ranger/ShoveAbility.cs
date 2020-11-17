using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoveAbility : Ability
{
    public ShoveAbilityData shoveAbilityData => (ShoveAbilityData)data;
    public override Type dataType => typeof(ShoveAbilityData);

    [HideInInspector]
    public GameHex.Dir direction;
    [HideInInspector]
    bool dontShove = false;

    public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability data) {
        return new InstantAbilityHandler(data, this, (ability) => {
            dontShove = false;
            var target = Game.grid.hexes[data.target];
            var aoe = GetAffectedArea(target);
            foreach (var hex in aoe) {
                if (hex.unit) {
                    hex.unit.gameObject.AddEntityComponent(shoveAbilityData.rootModifier);
                    hex.unit.MovePosition(GetTargetHex(hex));
                }
            }
        });
    }

    GameHex GetTargetHex(GameHex hex) {
        direction = CheckDirection(hex);
        if (!dontShove) {
            if (direction == GameHex.Dir.DownRight) {
                while (hex.downRight != null && !hex.downRight.unit && !hex.downRight.blocked) {
                    hex = hex.downRight;
                }
            }
            if (direction == GameHex.Dir.DownLeft) {
                while (hex.downLeft != null && !hex.downLeft.unit && !hex.downLeft.blocked) {
                    hex = hex.downLeft;
                }
            }
            if (direction == GameHex.Dir.Left) {
                while (hex.left != null && !hex.left.unit && !hex.left.blocked) {
                    hex = hex.left;
                }
            }
            if (direction == GameHex.Dir.UpLeft) {
                while (hex.upLeft != null && !hex.upLeft.unit && !hex.upLeft.blocked) {
                    hex = hex.upLeft;
                }
            }
            if (direction == GameHex.Dir.UpRight) {
                while (hex.upRight != null && !hex.upRight.unit && !hex.upRight.blocked) {
                    hex = hex.upRight;
                }
            }
            if (direction == GameHex.Dir.Right) {
                while (hex.right != null && !hex.right.unit && !hex.right.blocked) {
                    hex = hex.right;
                }
            }
        }
        return hex;
    }


    GameHex.Dir CheckDirection(GameHex hex) {
        var u = unit;
        if (hex.upLeft.unit && hex.upLeft.unit == u) {
            return GameHex.Dir.DownRight;
        }
        if (hex.upRight.unit && hex.upRight.unit == u) {
            return GameHex.Dir.DownLeft;
        }
        if (hex.right.unit && hex.right.unit == u) {
            return GameHex.Dir.Left;
        }
        if (hex.downRight.unit && hex.downRight.unit == u) {
            return GameHex.Dir.UpLeft;
        }
        if (hex.downLeft.unit && hex.downLeft.unit == u) {
            return GameHex.Dir.UpRight;
        }
        if (hex.left.unit && hex.left.unit == u) {
            return GameHex.Dir.Right;
        } else {
            Debug.LogError("Unit was not in melee range! Something has gone wrong!");
            dontShove = true;
            return GameHex.Dir.Right;
        }
    }
}
