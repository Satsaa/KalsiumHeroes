﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShrapnelAbility : Ability {

	public new ShrapnelAbilityData data => (ShrapnelAbilityData)_data;
	public override Type dataType => typeof(ShrapnelAbilityData);

	public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var target = Game.grid.tiles[msg.targets.First()];
			var modifier = (ShrapnelAbilityModifier)UnitModifier.Create(master, data.shrapnelModifierData);

			modifier.target = target;
			modifier.creator = this;
			modifier.aoe = base.GetAffectedArea(target).ToList();

			modifier.calculatedDamage = data.damage.value;
			modifier.damageType = data.damageType;

			CalculateDamage(ref modifier.calculatedDamage, ref modifier.damageType);
		});
	}

}